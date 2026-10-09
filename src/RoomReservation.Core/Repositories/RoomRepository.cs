using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Extensions;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;

namespace RoomReservation.Core.Repositories
{
    public class RoomRepository(AppDbContext db, TimeProvider timeProvider) : IRoomRepository
    {
        public void Add(Room room)
            => db.Rooms.Add(room);

        public void Remove(Room room)
            => db.Rooms.Remove(room);

        public async Task<bool> ExistsByIdentifierAsync(Guid buildingId, string identifier, Guid? excludeId = null)
        {
            var query = db.Rooms.Where(r => r.BuildingId == buildingId && r.Identifier == identifier);
            if (excludeId.HasValue)
            {
                query = query.Where(r => r.Id != excludeId.Value);
            }
            return await query.AnyAsync();
        }

        public async Task<Room?> GetByIdAsync(Guid roomId)
        {
            var today = timeProvider.WarsawToday();

            return await db.Rooms.Include(r => r.Building)
                                 .Include(r => r.RoomEquipment)
                                    .ThenInclude(re => re.Equipment)
                                 .Include(r => r.Availabilities)
                                 .Include(r => r.Reservations.Where(res =>
                                        res.Date >= today &&
                                        (res.Status == ReservationStatus.Approved || res.Status == ReservationStatus.Pending)))
                                    .ThenInclude(res => res.CreatedBy)
                                 .Include(r => r.Events.Where(e => e.EndDate >= today))
                                    .ThenInclude(e => e.Rooms)
                                 .AsSplitQuery()
                                 .FirstOrDefaultAsync(r => r.Id == roomId);
        }

        public async Task<Room?> GetByIdentifierAsync(Guid buildingId, string identifier)
        {
            return await db.Rooms.FirstOrDefaultAsync(r => r.BuildingId == buildingId && r.Identifier == identifier);
        }

        public async Task<PagedList<Room>> GetFilteredAsync(RoomFilter filters)
        {
            var rooms = db.Rooms.AsNoTracking()
                                .Include(r => r.Building)
                                .Include(r => r.RoomEquipment)
                                    .ThenInclude(re => re.Equipment)
                                .Include(r => r.Availabilities)
                                .AsSplitQuery();

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
                rooms = rooms.Where(r => r.Availabilities.Any(ra =>
                    (!filters.DayOfWeek.HasValue || ra.DayOfWeek == filters.DayOfWeek.Value) &&
                    (!filters.StartTime.HasValue || ra.StartTime <= filters.StartTime.Value) &&
                    (!filters.EndTime.HasValue || ra.EndTime >= filters.EndTime.Value)));
            }

            if (filters.EquipmentIds is { Count: > 0 })
            {
                rooms = rooms.Where(r => filters.EquipmentIds.All(eid => r.RoomEquipment.Any(re => re.EquipmentId == eid)));
            }

            return await rooms
                .OrderBy(r => r.Identifier)
                .ThenBy(r => r.Id)
                .ToPagedListAsync(filters);
        }

        public async Task<IReadOnlyList<Room>> GetByIdsAsync(IReadOnlyList<Guid> roomIds)
        {
            var rooms = await db.Rooms
                .Include(r => r.Building)
                .Where(r => roomIds.Contains(r.Id))
                .ToListAsync();
            return rooms;
        }

        public async Task<IReadOnlyList<Room>> GetByBuildingIdAsync(Guid buildingId)
        {
            var rooms = await db.Rooms
                .Include(r => r.Availabilities)
                .Where(r => r.BuildingId == buildingId)
                .ToListAsync();
            return rooms;
        }
    }
}
