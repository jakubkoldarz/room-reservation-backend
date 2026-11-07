using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Extensions;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;

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

        public async Task<PagedList<Event>> GetFilteredAsync(EventFilter filters)
        {
            var events = db.Events
                .AsNoTracking()
                .Include(e => e.Rooms)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filters.Name))
                events = events.Where(e => EF.Functions.ILike(e.Name, $"%{filters.Name.Trim()}%"));

            if (filters.RoomId.HasValue)
                events = events.Where(e => e.Rooms.Any(r => r.Id == filters.RoomId));

            if (filters.BuildingId.HasValue)
                events = events.Where(e => e.Rooms.Any(r => r.BuildingId == filters.BuildingId));

            if (filters.IsClosed.HasValue)
                events = events.Where(e => e.IsClosed == filters.IsClosed);

            if (filters.DateFrom.HasValue)
                events = events.Where(e => e.EndDate >= filters.DateFrom);

            if (filters.DateTo.HasValue)
                events = events.Where(e => e.StartDate <= filters.DateTo);

            return await events
                .OrderBy(e => e.StartDate)
                .ThenBy(e => e.Name)
                .ThenBy(e => e.Id)
                .ToPagedListAsync(filters);
        }

        public async Task<Event?> GetByIdAsync(Guid id)
        {
            var ev = await db.Events.Include(e => e.Rooms).FirstOrDefaultAsync(e => e.Id == id);
            return ev;
        }
    }
}
