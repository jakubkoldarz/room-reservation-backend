using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;

namespace RoomReservation.Core.Interfaces
{
    public interface IRoomRepository
    {
        Task<Room?> GetByIdAsync(Guid roomId);
        Task<Room?> GetByIdentifierAsync(Guid buildingId, string identifier);
        Task<bool> ExistsByIdentifierAsync(Guid buildingId, string identifier);
        Task<(IReadOnlyList<Room> Rooms, int TotalCount)> GetFilteredAsync(RoomFilter filters);
        Task AddAsync(Room room);
        Task UpdateAsync(Room room);
        Task DeleteAsync(Room room);
        Task<bool> DeleteSpecialAvailabilityByIdAsync(Guid specialAvailabilityId);
        Task AddSpecialAvailabilityAsync(RoomSpecialAvailability specialAvailability);
        Task<RoomAvailability?> GetAvailabilityByDateAsync(Guid roomId, DateOnly dateOnly);
        Task<RoomSpecialAvailability?> GetSpecialAvailabilityByDateAsync(Guid roomId, DateOnly dateOnly);
    }
}
