using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Models;

namespace RoomReservation.Core.Interfaces
{
    public interface IBuildingRepository
    {
        Task<Building?> GetByIdAsync(Guid buildingId);
        Task<Building?> GetByNameAsync(string name);
        Task<PagedList<Building>> GetFilteredAsync(BuildingFilter filters);
        Task<IReadOnlyList<Building>> GetAllAsync();
        Task<bool> ExistsByNameAsync(string name);
        void Add(Building building);
        void Remove(Building building);
    }
}
