using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Core.Repositories
{
    public class EquipmentRepository(AppDbContext _db) : IEquipmentRepository
    {
        public async Task AddAsync(Equipment equipment)
        {
            _db.Equipment.Add(equipment);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Equipment equipment)
        {
            _db.Equipment.Remove(equipment);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await _db.Equipment.AnyAsync(e => e.Name == name);
        }

        public async Task<(IReadOnlyList<Equipment> Equipments, int TotalCount)> GetAllAsync(EquipmentFilter filters)
        {
            var equipmentQuery = _db.Equipment.AsQueryable();

            if(!string.IsNullOrEmpty(filters.Name))
            {
                equipmentQuery = equipmentQuery.Where(e => EF.Functions.ILike(e.Name, $"%{filters.Name}%"));
            }

            var totalCount = await equipmentQuery.CountAsync();

            var filteredEquipments = await equipmentQuery
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            return (filteredEquipments, totalCount);
        }

        public async Task<Equipment?> GetByIdAsync(Guid equipmentId)
        {
            return await _db.Equipment.FindAsync(equipmentId);
        }

        public async Task<Equipment?> GetByNameAsync(string name)
        {
            return await _db.Equipment.FirstOrDefaultAsync(e => e.Name == name);
        }

        public async Task UpdateAsync(Equipment equipment)
        {
            _db.Equipment.Update(equipment);
            await _db.SaveChangesAsync();
        }
    }
}
