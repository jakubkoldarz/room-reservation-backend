using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Interfaces
{
    public interface IEventRepository
    {
        Task<Event?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<Event>> GetActiveByRoomAsync(Guid roomId);
        Task<IReadOnlyList<Event>> GetActiveByRoomIdsAsync(IReadOnlyList<Guid> roomIds);
        Task AddAsync(Event ev);
        Task UpdateAsync(Event ev);
        Task DeleteAsync(Event ev);
    }
}
