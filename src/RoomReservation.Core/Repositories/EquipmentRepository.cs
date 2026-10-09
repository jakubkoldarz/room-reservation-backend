using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Extensions;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;

namespace RoomReservation.Core.Repositories
{
    public class EquipmentRepository(AppDbContext db) : IEquipmentRepository
    {
        public void Add(Equipment equipment)
            => db.Equipment.Add(equipment);

        public void Remove(Equipment equipment)
            => db.Equipment.Remove(equipment);

        public async Task<bool> AllExistAsync(IReadOnlyList<Guid> equipmentIds)
        {
            if (equipmentIds.Count == 0) return true;
            var existingCount = await db.Equipment.CountAsync(e => equipmentIds.Contains(e.Id));
            return existingCount == equipmentIds.Count;
        }

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await db.Equipment.AnyAsync(e => e.Name == name);
        }

        public async Task<PagedList<Equipment>> GetAllAsync(EquipmentFilter filters)
        {
            var equipmentQuery = db.Equipment.AsNoTracking();

            if(!string.IsNullOrEmpty(filters.Name))
            {
                equipmentQuery = equipmentQuery.Where(e => EF.Functions.ILike(e.Name, $"%{filters.Name}%"));
            }

            return await equipmentQuery
                .OrderBy(e => e.Name)
                .ThenBy(e => e.Id)
                .ToPagedListAsync(filters);
        }

        public async Task<Equipment?> GetByIdAsync(Guid equipmentId)
        {
            return await db.Equipment.FindAsync(equipmentId);
        }

        public async Task<Equipment?> GetByNameAsync(string name)
        {
            return await db.Equipment.FirstOrDefaultAsync(e => e.Name == name);
        }
    }
}
