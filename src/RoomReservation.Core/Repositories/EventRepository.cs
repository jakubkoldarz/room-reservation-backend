using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Core.Repositories
{
    public class EventRepository(AppDbContext _db) : IEventRepository
    {
        public async Task AddAsync(Event ev)
        {
            _db.Events.Add(ev);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(Event ev)
        {
            _db.Events.Remove(ev);
            await _db.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Event>> GetActiveByRoomAsync(Guid roomId)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

            return await _db.Events
                .Where(e => e.Rooms.Any(r => r.Id == roomId) && e.EndDate >= today)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Event>> GetActiveByRoomIdsAsync(IReadOnlyList<Guid> roomIds)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow.Date);

            return await _db.Events
                .Where(e => e.Rooms.Any(r => roomIds.Contains(r.Id)) && e.EndDate >= today)
                .ToListAsync();
        }

        public async Task<Event?> GetByIdAsync(Guid id)
        {
            var ev = await _db.Events.Include(e => e.Rooms).FirstOrDefaultAsync(e => e.Id == id);
            return ev;
        }

        public async Task UpdateAsync(Event ev)
        {
            _db.Events.Update(ev);
            await _db.SaveChangesAsync();
        }
    }
}
