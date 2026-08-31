using RoomReservation.Core.Entities;
using RoomReservation.Core.Models.Availability;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Interfaces
{
    public interface IEventService
    {
        bool AreEventsValid(IReadOnlyList<Guid> roomIds, DateOnly startDate, DateOnly endDate, IReadOnlyList<Event> existingEvents, Guid? excludeEventId = null);

        Task<ResultT<Event>> GetByIdAsync(Guid eventId);
        Task<IReadOnlyList<Event>> GetActiveForRoomAsync(Guid roomId);

        Task<ResultT<Event>> CreateAsync(IReadOnlyList<Guid> roomIds, EventModel request, bool force = false);
        Task<ResultT<Event>> UpdateAsync(Guid eventId, IReadOnlyList<Guid> roomIds, EventModel request, bool force = false);
        Task<Result> DeleteAsync(Guid eventId, bool force = false);
    }
}
