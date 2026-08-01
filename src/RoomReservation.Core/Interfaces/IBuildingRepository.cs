using RoomReservation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Interfaces
{
    public interface IBuildingRepository
    {
        Task<Building?> GetByIdAsync(Guid buildingId);
        Task<Building?> GetByNameAsync(string name);
        Task<IReadOnlyList<Building>> GetAllAsync();
        Task<bool> ExistsByNameAsync(string name);
        Task AddAsync(Building building);
        Task UpdateAsync(Building building);
        Task DeleteAsync(Building building);
    }
}
