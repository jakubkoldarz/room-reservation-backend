using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
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
            return await _db.Buildings.Include(b => b.Rooms).FirstOrDefaultAsync(b => b.Id == buildingId);
        }
        public async Task<Building?> GetByNameAsync(string name)
        {
            return await _db.Buildings.FirstOrDefaultAsync(b => b.Name.ToLower().Trim() == name.ToLower().Trim());
        }
        public async Task<(IReadOnlyList<Building> Buildings, int TotalCount)> GetFilteredAsync(BuildingFilter filters)
        {
            var buildings = _db.Buildings.AsQueryable();

            if(!string.IsNullOrWhiteSpace(filters.Name))
                buildings = buildings.Where(b => EF.Functions.ILike(b.Name, $"%{filters.Name.Trim()}%"));
            if (!string.IsNullOrWhiteSpace(filters.Identifier))
                buildings = buildings.Where(b => (string.IsNullOrWhiteSpace(b.Identifier) == false) && EF.Functions.ILike(b.Identifier, $"%{filters.Identifier.Trim()}%"));
            if (!string.IsNullOrWhiteSpace(filters.Street))
                buildings = buildings.Where(b => EF.Functions.ILike(b.Street, $"%{filters.Street.Trim()}%"));
            if (!string.IsNullOrWhiteSpace(filters.City))
                buildings = buildings.Where(b => EF.Functions.ILike(b.City, $"%{filters.City.Trim()}%"));
            if (!string.IsNullOrWhiteSpace(filters.PostalCode))
                buildings = buildings.Where(b => EF.Functions.ILike(b.PostalCode, $"%{filters.PostalCode.Trim()}%"));

            var totalCount = await buildings.CountAsync();

            var filteredBuildings = await buildings
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            return (filteredBuildings, totalCount);
        }
        public async Task<IReadOnlyList<Building>> SearchByNameAsync(string name)
        {
            return await _db.Buildings
                .Where(b => EF.Functions.ILike(b.Name, $"%{name.Trim()}%"))
                .ToListAsync();
        }
        public async Task UpdateAsync(Building building)
        {
            _db.Buildings.Update(building);
            await _db.SaveChangesAsync();
        }
    }
}
