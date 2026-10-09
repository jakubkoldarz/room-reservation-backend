using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Extensions;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Core.Repositories
{
    public class EventRepository(AppDbContext db, TimeProvider timeProvider) : IEventRepository
    {
        public void Add(Event ev)
            => db.Events.Add(ev);

        public void Remove(Event ev)
            => db.Events.Remove(ev);

        public async Task<IReadOnlyList<Event>> GetActiveByRoomAsync(Guid roomId)
        {
            var today = timeProvider.WarsawToday();

            return await db.Events
                .Include(e => e.Rooms)
                .Where(e => e.Rooms.Any(r => r.Id == roomId) && e.EndDate >= today)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Event>> GetActiveByRoomIdsAsync(IReadOnlyList<Guid> roomIds)
        {
            var today = timeProvider.WarsawToday();

            return await db.Events
                .Include(e => e.Rooms)
                .Where(e => e.Rooms.Any(r => roomIds.Contains(r.Id)) && e.EndDate >= today)
                .ToListAsync();
        }

        public async Task<Event?> GetByIdAsync(Guid id)
        {
            var ev = await db.Events.Include(e => e.Rooms).FirstOrDefaultAsync(e => e.Id == id);
            return ev;
        }
    }
}
