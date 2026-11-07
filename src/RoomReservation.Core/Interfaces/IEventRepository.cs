using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Models;

namespace RoomReservation.Core.Interfaces
{
    public interface IEventRepository
    {
        Task<Event?> GetByIdAsync(Guid id);
        Task<PagedList<Event>> GetFilteredAsync(EventFilter filters);
        Task<IReadOnlyList<Event>> GetActiveByRoomAsync(Guid roomId);
        Task<IReadOnlyList<Event>> GetActiveByRoomIdsAsync(IReadOnlyList<Guid> roomIds);
        void Add(Event ev);
        void Remove(Event ev);
    }
}
