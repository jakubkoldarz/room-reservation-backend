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
        public async Task<Result> AreAvailabilitiesValid(
            IReadOnlyList<Availability> availabilities,
            Guid? boundingBuildingId = null)
        {
            if (availabilities.Count == 0)
                return Result.Success();

            if (availabilities.Any(a => a.StartTime >= a.EndTime))
                return new Error("Invalid availability: start time is not before end time", ErrorType.BadRequest);

            bool hasDuplicateDays = availabilities.Select(a => a.DayOfWeek).Distinct().Count() != availabilities.Count;
            if (hasDuplicateDays)
                return new Error("Invalid availability: duplicate days found", ErrorType.Conflict);

            if (boundingBuildingId is not null)
            {
                var boundingAvailabilities = await _availabilities.GetByBuildingAsync(boundingBuildingId.Value);

                var fitsWithinBounds = IsWithinBuildingBounds(availabilities, boundingAvailabilities);
                if (!fitsWithinBounds)
                    return new Error("Invalid availability: does not fit within building bounds", ErrorType.Conflict);
            }

            return Result.Success();
        }

        public async Task<IReadOnlyList<Availability>> GetAllForRoomAsync(Guid roomId)
        {
            return await _availabilities.GetByRoomAsync(roomId);
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

        public bool IsWithinBuildingBounds(IReadOnlyList<Availability> roomAvailabilities, IReadOnlyList<Availability> buildingAvailabilities)
        {
            foreach (var room in roomAvailabilities)
            {
                var building = buildingAvailabilities.FirstOrDefault(b => b.DayOfWeek == room.DayOfWeek);
                if (building is null) return false;

                if (room.StartTime < building.StartTime || room.EndTime > building.EndTime)
                    return false;
            }
            return true;
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

        public async Task<IReadOnlyList<Room>> GetConflictingRoomsAsync(Guid buildingId, IReadOnlyList<Availability> newAvailabilities)
        {
            var allRooms = await _rooms.GetByBuildingIdAsync(buildingId);
            if (!allRooms.Any())
                return [];

            var conflictingRooms = new List<Room>();
            foreach (var room in allRooms)
            {
                var isWithinBounds = IsWithinBuildingBounds([.. room.Availabilities], newAvailabilities);
                if (!isWithinBounds)
                    conflictingRooms.Add(room);
            }
            return conflictingRooms;
        }

        public async Task<ResultT<IReadOnlyList<Availability>>> ReplaceIfValidForRoomAsync(Room room, IReadOnlyList<AvailabilityModel> models, bool force = false)
        {
            var newAvailabilities = models.Select(a => new Availability
            {
                RoomId = room.Id,
                DayOfWeek = a.DayOfWeek,
                StartTime = a.StartTime,
                EndTime = a.EndTime
            }).ToList();

            var validationResult = await AreAvailabilitiesValid(newAvailabilities, boundingBuildingId: room.BuildingId);
            if (!validationResult.IsSuccess)
                return validationResult.Error;

            var events = await _events.GetActiveByRoomAsync(room.Id);
            var conflicts = await GetConflictingReservationsForRoomAsync(room.Id, newAvailabilities, events);
            if (!force && conflicts.Any())
                return new Error("Conflicting reservations found", ErrorType.Conflict);

            await _availabilities.ReplaceForRoomAsync(room.Id, newAvailabilities);
            return ResultT<IReadOnlyList<Availability>>.Success(newAvailabilities);
        }

        public async Task<ResultT<IReadOnlyList<Availability>>> ReplaceIfValidForBuildingAsync(Building building, IReadOnlyList<AvailabilityModel> availabilityModels)
        {
            var availabilities = availabilityModels.Select(a => new Availability
            {
                BuildingId = building.Id,
                DayOfWeek = a.DayOfWeek,
                StartTime = a.StartTime,
                EndTime = a.EndTime
            }).ToList();

            var validationResult = await AreAvailabilitiesValid(availabilities);
            if (!validationResult.IsSuccess)
                return validationResult.Error;

            var conflictingRoomAvailabilities = await GetConflictingRoomsAsync(building.Id, availabilities);
            if (conflictingRoomAvailabilities.Any())
                return new Error("Some rooms have conflicting availabilities", ErrorType.Conflict);

            await _availabilities.ReplaceForBuildingAsync(building.Id, availabilities);
            return ResultT<IReadOnlyList<Availability>>.Success(availabilities);
        }
    }
}