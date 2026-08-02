using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Interfaces
{
    public interface IRoomRepository
    {
        Task<Room?> GetByIdAsync(Guid roomId);
        Task AddAsync(Room room);
        Task UpdateAsync(Room room);
        Task DeleteAsync(Guid roomId);
    }
}
