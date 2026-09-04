using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;

namespace RoomReservation.Core.Interfaces
{
    public interface IBuildingRepository
    {
        Task<Building?> GetByIdAsync(Guid buildingId);
        Task<Building?> GetByNameAsync(string name);
        Task<(IReadOnlyList<Building> Buildings, int TotalCount)> GetFilteredAsync(BuildingFilter filters);
        Task<IReadOnlyList<Building>> GetAllAsync();
        Task<bool> ExistsByNameAsync(string name);
        Task AddAsync(Building building);
        Task UpdateAsync(Building building);
        Task DeleteAsync(Building building);
    }
}
