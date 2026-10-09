using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Core.Repositories
{
    public class AvailabilityRepository(AppDbContext db) : IAvailabilityRepository
    {
        public async Task<IReadOnlyList<Availability>> GetByBuildingAsync(Guid buildingId)
        {
            var availabilities = await db.Availabilities
                .Where(a => a.BuildingId == buildingId)
                .ToListAsync();

            return availabilities;
        }

        public async Task<IReadOnlyList<Availability>> GetByRoomAsync(Guid roomId)
        {
            var availabilities = await db.Availabilities
                .Where(a => a.RoomId == roomId)
                .ToListAsync();

            return availabilities;
        }

        public async Task<IReadOnlyList<Availability>> GetByRoomIdsAsync(IReadOnlyList<Guid> roomIds)
        {
            var availabilities = await db.Availabilities
                .Where(a => roomIds.Contains(a.RoomId!.Value))
                .ToListAsync();

            return availabilities;
        }

        public async Task ReplaceForRoomAsync(Guid roomId, IReadOnlyList<Availability> availabilities)
        {
            var existing = await db.Availabilities
                .Where(a => a.RoomId == roomId)
                .ToListAsync();

            db.Availabilities.RemoveRange(existing);
            db.Availabilities.AddRange(availabilities);
        }

        public async Task ReplaceForBuildingAsync(Guid buildingId, IReadOnlyList<Availability> availabilities)
        {
            var existing = await db.Availabilities
                .Where(a => a.BuildingId == buildingId)
                .ToListAsync();

            db.Availabilities.RemoveRange(existing);
            db.Availabilities.AddRange(availabilities);
        }
    }
}
