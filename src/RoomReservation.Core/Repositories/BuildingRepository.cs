using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Core.Repositories
{
    public class BuildingRepository(AppDbContext _db) : IBuildingRepository
    {
        public async Task AddAsync(Building building)
        {
            _db.Buildings.Add(building);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Building building)
        {
            _db.Buildings.Remove(building);
            await _db.SaveChangesAsync();
        }

        public Task<bool> ExistsByNameAsync(string name)
        {
            return _db.Buildings.AnyAsync(b => b.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        public async Task<IReadOnlyList<Building>> GetAllAsync()
        {
            return await _db.Buildings.ToListAsync();
        }

        public async Task<Building?> GetByIdAsync(Guid buildingId)
        {
            return await _db.Buildings.FindAsync(buildingId);
        }

        public async Task<Building?> GetByNameAsync(string name)
        {
            return await _db.Buildings.FirstOrDefaultAsync(b => b.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        public async Task UpdateAsync(Building building)
        {
            _db.Buildings.Update(building);
            await _db.SaveChangesAsync();
        }
    }
}
