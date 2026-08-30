using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models.Availability;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public class AvailabilityService(
        IAvailabilityRepository _availabilities,
        IReservationRepository _reservations,
        IEventRepository _events,
        IRoomRepository _rooms) : IAvailabilityService
    {
        public bool AreAvailabilitiesValid(IReadOnlyList<Availability> availabilities)
        {
            if (availabilities.Any(a => a.StartTime >= a.EndTime))
                return false;

            bool hasDuplicateDays = availabilities
                .GroupBy(a => a.DayOfWeek)
                .Any(group => group.Count() > 1);

            return !hasDuplicateDays;
        }

        public async Task<IReadOnlyList<Availability>> GetAllForRoomAsync(Guid roomId)
        {
            return await _availabilities.GetByRoomAsync(roomId);
        }

        public async Task<IReadOnlyList<Availability>> GetDefaultsForBuildingAsync(Guid buildingId)
        {
            return await _availabilities.GetByBuildingAsync(buildingId);
        }

        public async Task<ResultT<IReadOnlyList<Availability>>> ReplaceForRoomAsync(Guid roomId, IReadOnlyList<AvailabilityRequest> request, bool force = false)
        {
            var room = await _rooms.GetByIdAsync(roomId);
            if (room is null)
                return new Error("Room not found", ErrorType.NotFound);

            var newAvailabilities = request.Select(a => new Availability
            {
                RoomId = roomId,
                DayOfWeek = a.DayOfWeek,
                StartTime = a.StartTime,
                EndTime = a.EndTime
            }).ToList();

            if (!AreAvailabilitiesValid(newAvailabilities))
                return new Error("Invalid availabilities provided", ErrorType.BadRequest);

            var events = await _events.GetActiveByRoomAsync(roomId);
            var conflicts = await GetConflictingReservationsForRoomAsync(roomId, newAvailabilities, events);
            if (!force && conflicts.Any())
                return new Error("Conflicting reservations found", ErrorType.Conflict);

            await _availabilities.ReplaceForRoomAsync(roomId, newAvailabilities);
            return ResultT<IReadOnlyList<Availability>>.Success(newAvailabilities);
        }

        public async Task<AvailabilityResolution> ResolveAvailabilityAsync(Guid roomId, DateOnly date)
        {
            var availabilities = await _availabilities.GetByRoomAsync(roomId);
            var events = await _events.GetActiveByRoomAsync(roomId);

            return ResolveAvailability(roomId, date, availabilities, events);
        }

        public async Task<IReadOnlyList<Reservation>> GetConflictingReservationsForRoomAsync(Guid roomId, IReadOnlyList<Availability> availabilities, IReadOnlyList<Event> events)
        {
            var conflictingReservations = new List<Reservation>();
            var activeReservations = await _reservations.GetActiveFutureByRoomAsync(roomId);

            foreach (var reservation in activeReservations)
            {
                var resolution = ResolveAvailability(roomId, reservation.Date, availabilities, events);
                if (!IsWithinAvailability(reservation.StartTime, reservation.EndTime, resolution))
                    conflictingReservations.Add(reservation);
            }

            return conflictingReservations;
        }

        public async Task<IReadOnlyList<Reservation>> GetConflictingReservationsForRoomsAsync(IReadOnlyList<Guid> roomIds, IReadOnlyList<Availability> availabilities, IReadOnlyList<Event> events)
        {
            var conflictingReservations = new List<Reservation>();
            var activeReservations = await _reservations.GetActiveFutureByRoomIdsAsync(roomIds);
            var groupedReservations = activeReservations.GroupBy(r => r.RoomId);

            foreach (var roomGroup in groupedReservations)
            {
                var roomId = roomGroup.Key;

                foreach (var reservation in roomGroup)
                {
                    var resolution = ResolveAvailability(roomId, reservation.Date, availabilities, events);
                    if (!IsWithinAvailability(reservation.StartTime, reservation.EndTime, resolution))
                        conflictingReservations.Add(reservation);
                }
            }

            return conflictingReservations;
        }

        private static AvailabilityResolution ResolveAvailability(
            Guid roomId,
            DateOnly date,
            IReadOnlyList<Availability> availabilities,
            IReadOnlyList<Event> events)
        {
            var activeEvent = events.FirstOrDefault(e =>
                e.Rooms.Any(r => r.Id == roomId) && e.StartDate <= date && e.EndDate >= date);

            if (activeEvent is not null)
            {
                if (activeEvent.IsClosed) return new AvailabilityResolution(true, null, null);
                return new AvailabilityResolution(false, activeEvent.StartTime, activeEvent.EndTime);
            }

            var availability = availabilities.FirstOrDefault(a => a.RoomId == roomId && a.DayOfWeek == date.DayOfWeek);
            if (availability is not null)
                return new AvailabilityResolution(false, availability.StartTime, availability.EndTime);

            return new AvailabilityResolution(true, null, null);
        }

        private static bool IsWithinAvailability(TimeOnly start, TimeOnly end, AvailabilityResolution resolution)
        {
            if (resolution.IsClosed) return false;
            return start >= resolution.StartTime!.Value && end <= resolution.EndTime!.Value;
        }
    }
}