using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models.Availability;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public class EventService(
        IEventRepository _events,
        IAvailabilityRepository _availabilities,
        IAvailabilityService _availabilityService,
        IRoomRepository _rooms) : IEventService
    {
        public bool AreEventsValid(IReadOnlyList<Guid> roomIds, DateOnly startDate, DateOnly endDate, IReadOnlyList<Event> existingEvents, Guid? excludeEventId = null)
        {
            var relevantEvents = existingEvents.Where(e => excludeEventId == null || e.Id != excludeEventId);

            foreach (var existingEvent in relevantEvents)
            {
                bool noDateOverlap = endDate < existingEvent.StartDate || existingEvent.EndDate < startDate;
                if (noDateOverlap)
                    continue;

                bool sharesRoom = existingEvent.Rooms.Any(r => roomIds.Contains(r.Id));
                if (sharesRoom)
                    return false;
            }

            return true;
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
            if (!AreEventsValid(roomIds, request.StartDate, request.EndDate, existingEvents))
                return new Error("One or more rooms already have an overlapping event", ErrorType.Conflict);

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

            var combinedEvents = existingEvents.Append(newEvent).ToList();
            var availabilities = await _availabilities.GetByRoomIdsAsync(roomIds); 

            var conflicts = await _availabilityService.GetConflictingReservationsForRoomsAsync(roomIds, availabilities, combinedEvents);
            if (!force && conflicts.Any())
                return new Error("Conflicting reservations found", ErrorType.Conflict);

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
                return new Error("Conflicting reservations found", ErrorType.Conflict);

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

            toUpdate.Name = request.Name;
            toUpdate.StartDate = request.StartDate;
            toUpdate.EndDate = request.EndDate;
            toUpdate.IsClosed = request.IsClosed;
            toUpdate.StartTime = request.StartTime;
            toUpdate.EndTime = request.EndTime;
            toUpdate.Rooms = [.. rooms];

            var otherEvents = await _events.GetActiveByRoomIdsAsync(roomIds);
            if (!AreEventsValid(roomIds, request.StartDate, request.EndDate, otherEvents, excludeEventId: eventId))
                return new Error("One or more rooms already have an overlapping event", ErrorType.Conflict);

            var combinedEvents = otherEvents.Where(e => e.Id != eventId).Append(toUpdate).ToList();
            var availabilities = await _availabilities.GetByRoomIdsAsync(roomIds);

            var conflicts = await _availabilityService.GetConflictingReservationsForRoomsAsync(roomIds, availabilities, combinedEvents);
            if (!force && conflicts.Any())
                return new Error("Conflicting reservations found", ErrorType.Conflict);

            await _events.UpdateAsync(toUpdate);
            return ResultT<Event>.Success(toUpdate);
        }

        private static Result IsEventValid(EventModel request)
        {
            if (request.StartDate > request.EndDate)
                return new Error("Invalid date range", ErrorType.BadRequest);

            if (request.IsClosed && (request.StartTime is not null || request.EndTime is not null))
                return new Error("Cannot specify start or end times for closed event", ErrorType.BadRequest);

            if (!request.IsClosed && (request.StartTime is null || request.EndTime is null))
                return new Error("Missing start or end time for open event", ErrorType.BadRequest);

            return Result.Success();
        }
    }
}
