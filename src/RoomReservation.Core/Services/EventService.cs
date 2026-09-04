using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models.Events;
using RoomReservation.Core.Models.Reservations;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public class EventService(
        IEventRepository _events,
        IAvailabilityRepository _availabilities,
        IAvailabilityService _availabilityService,
        IReservationService _reservationService,
        IRoomRepository _rooms) : IEventService
    {
        public IReadOnlyList<Event> GetConflictingEvents(IReadOnlyList<Guid> roomIds, DateOnly startDate, DateOnly endDate, IReadOnlyList<Event> existingEvents, Guid? excludeEventId = null)
        {
            var relevantEvents = existingEvents.Where(e => excludeEventId == null || e.Id != excludeEventId);

            var conflictingEvents = new List<Event>();

            foreach (var existingEvent in relevantEvents)
            {
                bool noDateOverlap = endDate < existingEvent.StartDate || existingEvent.EndDate < startDate;
                if (noDateOverlap)
                    continue;

                bool sharesRoom = existingEvent.Rooms.Any(r => roomIds.Contains(r.Id));
                if (sharesRoom)
                    conflictingEvents.Add(existingEvent);
            }

            return conflictingEvents;
        }

        public async Task<ResultT<Event>> CreateAsync(IReadOnlyList<Guid> roomIds, EventModel request, bool force = false)
        {
            var rooms = await _rooms.GetByIdsAsync(roomIds);
            if (rooms.Count != roomIds.Count)
                return new Error("One or more rooms not found", ErrorType.NotFound);

            var validationResult = IsEventValid(request);
            if (!validationResult.IsSuccess)
                return validationResult.Error;

            var existingEvents = await _events.GetActiveByRoomIdsAsync(roomIds);
            var conflictingEvents = GetConflictingEvents(roomIds, request.StartDate, request.EndDate, existingEvents);
            if (conflictingEvents.Any())
            {
                var conflictingEventModels = conflictingEvents.Select(e => new ConflictingEventModel(e.Id, e.Name, e.StartDate, e.EndDate)).ToList();
                return new ConflictError<ConflictingEventModel>("One or more rooms already have an overlapping event", conflictingEventModels);
            }

            var newEvent = new Event
            {
                Name = request.Name,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsClosed = request.IsClosed,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Rooms = [.. rooms],
            };

            var conflictingReservations = await _reservationService.GetConflictingWithEventAsync(newEvent);
            if (!force && conflictingReservations.Any())
            {
                var conflictingModels = conflictingReservations
                    .Select(r => new ConflictingReservationModel(r.Id, r.Date, r.StartTime, r.EndTime, r.RoomId, r.Status.ToString().ToUpper())).ToList();
                return new ConflictError<ConflictingReservationModel>("Conflicting reservations found", conflictingModels);
            }

            await _events.AddAsync(newEvent);
            return ResultT<Event>.Success(newEvent);
        }

        public async Task<Result> DeleteAsync(Guid eventId, bool force = false)
        {
            var existingEvent = await _events.GetByIdAsync(eventId);
            if (existingEvent is null)
                return new Error("Event not found", ErrorType.NotFound);

            var roomIds = existingEvent.Rooms.Select(r => r.Id).ToList();

            var otherEvents = await _events.GetActiveByRoomIdsAsync(roomIds);
            var remainingEvents = otherEvents.Where(e => e.Id != eventId).ToList();

            var availabilities = await _availabilities.GetByRoomIdsAsync(roomIds);

            var conflicts = await _availabilityService.GetConflictingReservationsForRoomsAsync(roomIds, availabilities, remainingEvents);
            if (!force && conflicts.Any())
            {
                var conflictingModels = conflicts
                    .Select(r => new ConflictingReservationModel(r.Id, r.Date, r.StartTime, r.EndTime, r.RoomId, r.Status.ToString().ToUpper())).ToList();
                return new ConflictError<ConflictingReservationModel>("Conflicting reservations found", conflictingModels);
            }

            await _events.DeleteAsync(existingEvent);
            return Result.Success();
        }

        public async Task<IReadOnlyList<Event>> GetActiveForRoomAsync(Guid roomId)
        {
            var activeEvents = await _events.GetActiveByRoomAsync(roomId);
            return activeEvents;
        }

        public async Task<ResultT<Event>> GetByIdAsync(Guid eventId)
        {
            var ev = await _events.GetByIdAsync(eventId);
            if (ev is null)
                return new Error("Event not found", ErrorType.NotFound);

            return ResultT<Event>.Success(ev);
        }

        public async Task<ResultT<Event>> UpdateAsync(Guid eventId, IReadOnlyList<Guid> roomIds, EventModel request, bool force = false)
        {
            var toUpdate = await _events.GetByIdAsync(eventId);
            if (toUpdate is null)
                return new Error("Event not found", ErrorType.NotFound);

            var rooms = await _rooms.GetByIdsAsync(roomIds);
            if (rooms.Count != roomIds.Count)
                return new Error("One or more rooms not found", ErrorType.NotFound);

            var validationResult = IsEventValid(request);
            if (!validationResult.IsSuccess)
                return validationResult.Error;

            var otherEvents = await _events.GetActiveByRoomIdsAsync(roomIds);
            var conflictingEvents = GetConflictingEvents(roomIds, request.StartDate, request.EndDate, otherEvents, excludeEventId: eventId);
            if (conflictingEvents.Any())
            {
                var conflictingEventModels = conflictingEvents.Select(e => new ConflictingEventModel(e.Id, e.Name, e.StartDate, e.EndDate)).ToList();
                return new ConflictError<ConflictingEventModel>("One or more rooms already have an overlapping event", conflictingEventModels);
            }

            var candidate = new Event
            {
                Id = toUpdate.Id,
                Name = request.Name,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsClosed = request.IsClosed,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Rooms = [.. rooms],
            };

            var conflictingReservations = await _reservationService.GetConflictingWithEventAsync(candidate);
            if (!force && conflictingReservations.Any())
            {
                var conflictingModels = conflictingReservations
                    .Select(r => new ConflictingReservationModel(r.Id, r.Date, r.StartTime, r.EndTime, r.RoomId, r.Status.ToString().ToUpper())).ToList();
                return new ConflictError<ConflictingReservationModel>("Conflicting reservations found", conflictingModels);
            }

            toUpdate.Name = request.Name;
            toUpdate.StartDate = request.StartDate;
            toUpdate.EndDate = request.EndDate;
            toUpdate.IsClosed = request.IsClosed;
            toUpdate.StartTime = request.StartTime;
            toUpdate.EndTime = request.EndTime;
            toUpdate.Rooms = [.. rooms];

            await _events.UpdateAsync(toUpdate);
            return ResultT<Event>.Success(toUpdate);
        }

        private static Result IsEventValid(EventModel request)
        {
            if (request.IsClosed && (request.StartTime is not null || request.EndTime is not null))
                return new Error("Cannot specify start or end times for closed event", ErrorType.BadRequest);

            if (!request.IsClosed && (request.StartTime is null || request.EndTime is null))
                return new Error("Missing start or end time for open event", ErrorType.BadRequest);

            if (request.StartDate > request.EndDate)
                return new Error("Start date is after end date", ErrorType.BadRequest);

            if (request.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
                return new Error("Start date is in the past", ErrorType.BadRequest);

            return Result.Success();
        }
    }
}
