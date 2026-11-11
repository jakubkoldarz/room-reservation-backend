using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Extensions;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;

namespace RoomReservation.Core.Repositories
{
    public class BuildingRepository(AppDbContext db) : IBuildingRepository
    {
        private static readonly SortMap<Building> Sorts = new SortMap<Building>("name")
            .Add("name", b => b.Name)
            .Add("city", b => b.City, b => b.Street)
            .Add("floors", b => b.FloorsCount);

        public void Add(Building building)
            => db.Buildings.Add(building);

        public void Remove(Building building)
            => db.Buildings.Remove(building);

        public Task<bool> ExistsByNameAsync(string name)
        {
            return db.Buildings.AnyAsync(b => b.Name.ToLower().Trim() == name.ToLower().Trim());
        }
        public async Task<IReadOnlyList<Building>> GetAllAsync()
        {
            return await db.Buildings
                .AsNoTracking()
                .OrderBy(b => b.Name)
                .ToListAsync();
        }
        public async Task<Building?> GetByIdAsync(Guid buildingId)
        {
            return await db.Buildings
                .Include(b => b.Rooms)
                .Include(b => b.Availabilities)
                .AsSplitQuery()
                .FirstOrDefaultAsync(b => b.Id == buildingId);
        }
        public async Task<Building?> GetByNameAsync(string name)
        {
            return await db.Buildings.FirstOrDefaultAsync(b => b.Name.ToLower().Trim() == name.ToLower().Trim());
        }
        public async Task<PagedList<Building>> GetFilteredAsync(BuildingFilter filters)
        {
            var buildings = db.Buildings.AsNoTracking();

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

            return await Sorts.Apply(buildings, filters)
                .ThenBy(b => b.Id)
                .ToPagedListAsync(filters);
        }

        public async Task<IReadOnlyList<Building>> SearchByNameAsync(string name)
        {
            return await db.Buildings
                .Where(b => EF.Functions.ILike(b.Name, $"%{name.Trim()}%"))
                .ToListAsync();
        }
    }
}
