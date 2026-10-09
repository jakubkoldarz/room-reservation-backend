using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Models;

namespace RoomReservation.Core.Interfaces
{
    public interface IRoomRepository
    {
        Task<Room?> GetByIdAsync(Guid roomId);
        Task<IReadOnlyList<Room>> GetByBuildingIdAsync(Guid buildingId);
        Task<IReadOnlyList<Room>> GetByIdsAsync(IReadOnlyList<Guid> roomIds);
        Task<Room?> GetByIdentifierAsync(Guid buildingId, string identifier);
        Task<bool> ExistsByIdentifierAsync(Guid buildingId, string identifier, Guid? excludeId = null);
        Task<PagedList<Room>> GetFilteredAsync(RoomFilter filters);
        void Add(Room room);
        void Remove(Room room);
    }
}
