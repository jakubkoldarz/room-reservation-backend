using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Core.Repositories
{
    public class AvailabilityRepository(AppDbContext _db) : IAvailabilityRepository
    {
        public async Task<IReadOnlyList<Availability>> GetByBuildingAsync(Guid buildingId)
        {
            var availabilities = await _db.Availabilities
                .Where(a => a.BuildingId == buildingId)
                .ToListAsync();

            return availabilities;
        }

        public async Task<IReadOnlyList<Availability>> GetByRoomAsync(Guid roomId)
        {
            var availabilities = await _db.Availabilities
                .Where(a => a.RoomId == roomId)
                .ToListAsync();

            return availabilities;
        }

        public async Task<IReadOnlyList<Availability>> GetByRoomIdsAsync(IReadOnlyList<Guid> roomIds)
        {
            var availabilities = await _db.Availabilities
                .Where(a => roomIds.Contains(a.RoomId!.Value))
                .ToListAsync();

            return availabilities;
        }

        public async Task ReplaceForRoomAsync(Guid roomId, IReadOnlyList<Availability> availabilities)
        {
            var existing = await _db.Availabilities
                .Where(a => a.RoomId == roomId)
                .ToListAsync();

            _db.Availabilities.RemoveRange(existing);
            await _db.Availabilities.AddRangeAsync(availabilities);

            await _db.SaveChangesAsync();
        }
    }
}
