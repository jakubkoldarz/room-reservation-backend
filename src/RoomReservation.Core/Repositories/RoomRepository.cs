using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Core.Repositories
{
    public class RoomRepository(AppDbContext _db) : IRoomRepository
    {
        public async Task AddAsync(Room room)
        {
            _db.Rooms.Add(room);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Room room)
        {
            _db.Rooms.Remove(room);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> ExistsByIdentifierAsync(Guid buildingId, string identifier)
        {
            return await _db.Rooms.AnyAsync(r => r.BuildingId == buildingId && r.Identifier == identifier);
        }

        public async Task<Room?> GetByIdAsync(Guid roomId)
        {
            return await _db.Rooms.Include(r => r.Building)
                                  .Include(r => r.RoomEquipment)
                                    .ThenInclude(re => re.Equipment)
                                  .Include(r => r.RoomAvailabilities)
                                  .FirstOrDefaultAsync(r => r.Id == roomId);
        }

        public async Task<Room?> GetByIdentifierAsync(Guid buildingId, string identifier)
        {
            return await _db.Rooms.FirstOrDefaultAsync(r => r.BuildingId == buildingId && r.Identifier == identifier);
        }

        public async Task UpdateAsync(Room room)
        {
            _db.Rooms.Update(room);
            await _db.SaveChangesAsync();
        }

        public async Task<(IReadOnlyList<Room> Rooms, int TotalCount)> GetFilteredAsync(RoomFilter filters)
        {
            var rooms = _db.Rooms.Include(r => r.Building)
                                  .Include(r => r.RoomEquipment)
                                    .ThenInclude(re => re.Equipment)
                                  .Include(r => r.RoomAvailabilities)
                                  .AsQueryable();

            if (filters.BuildingId.HasValue)
                rooms = rooms.Where(r => r.BuildingId == filters.BuildingId.Value);
            if (!string.IsNullOrEmpty(filters.Identifier))
                rooms = rooms.Where(r => EF.Functions.ILike(r.Identifier, $"%{filters.Identifier}%"));
            if (filters.MinCapacity.HasValue)
                rooms = rooms.Where(r => r.Capacity >= filters.MinCapacity.Value);
            if (filters.Floor.HasValue)
                rooms = rooms.Where(r => r.Floor == filters.Floor.Value);

            if (filters.DayOfWeek.HasValue || filters.StartTime.HasValue || filters.EndTime.HasValue)
            {
                rooms = rooms.Where(r => r.RoomAvailabilities.Any(ra =>
                    (!filters.DayOfWeek.HasValue || ra.DayOfWeek == filters.DayOfWeek.Value) &&
                    (!filters.StartTime.HasValue || ra.StartTime <= filters.StartTime.Value) &&
                    (!filters.EndTime.HasValue || ra.EndTime >= filters.EndTime.Value)));
            }

            if(filters.EquipmentIds != null && filters.EquipmentIds.Any())
            {
                rooms = rooms.Where(r => filters.EquipmentIds.All(eid => r.RoomEquipment.Any(re => re.EquipmentId == eid)));
            }

            var total = await rooms.CountAsync();
            var pagedRooms = await rooms.Skip((filters.Page - 1) * filters.PageSize)
                                       .Take(filters.PageSize)
                                       .ToListAsync();
            return (pagedRooms, total);
        }
    }
}
