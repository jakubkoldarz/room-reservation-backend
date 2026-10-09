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
    public class ReservationRepository(AppDbContext db, TimeProvider timeProvider) : IReservationRepository
    {
        public void Add(Reservation reservation)
            => db.Reservations.Add(reservation);

        public void Remove(Reservation reservation)
            => db.Reservations.Remove(reservation);

        public async Task LockRoomAsync(Guid roomId)
            => await db.Database.ExecuteSqlInterpolatedAsync(
                $@"SELECT 1 FROM ""Rooms"" WHERE ""Id"" = {roomId} FOR UPDATE");

        public async Task<IReadOnlyList<Reservation>> GetByIdsAsync(IReadOnlyList<Guid> reservationIds)
        {
            return await db.Reservations
                .Include(r => r.Room)
                    .ThenInclude(rm => rm.Building)
                .Include(r => r.CreatedBy)
                .Where(r => reservationIds.Contains(r.Id))
                .ToListAsync();
        }

        public async Task<Reservation?> GetByIdAsync(Guid reservationId)
        {
            var reservation = await db.Reservations
                .Include(r => r.Room)
                    .ThenInclude(rm => rm.Building)
                .Include(r => r.CreatedBy)
                .Include(r => r.ApprovedBy)
                .Include(r => r.CanceledBy)
                .Include(r => r.RejectedBy)
                .FirstOrDefaultAsync(r => r.Id == reservationId);

            return reservation;
        }
        public async Task<IReadOnlyList<Reservation>> GetActiveByRoomAndDateAsync(Guid roomId, DateOnly date)
        {
            var reservation = await db.Reservations
                .Where(r => 
                    r.RoomId == roomId && 
                    r.Date == date && 
                    (r.Status == ReservationStatus.Approved || r.Status == ReservationStatus.Pending))
                .ToListAsync();

            return reservation;
        }

        public async Task<PagedList<Reservation>> GetFilteredAsync(ReservationFilter filters)
        {
            var reservationsQuery = db.Reservations
                .AsNoTracking()
                .Include(r => r.Room)
                    .ThenInclude(rm => rm.Building)
                .Include(r => r.CreatedBy)
                .Include(r => r.ApprovedBy)
                .Include(r => r.CanceledBy)
                .Include(r => r.RejectedBy)
                .AsQueryable();

            if (filters.CreatedById.HasValue)
                reservationsQuery = reservationsQuery.Where(r => r.CreatedById == filters.CreatedById);

            if (filters.ApprovedById.HasValue)
                reservationsQuery = reservationsQuery.Where(r => r.ApprovedById == filters.ApprovedById);

            if (filters.CanceledById.HasValue)
                reservationsQuery = reservationsQuery.Where(r => r.CanceledById == filters.CanceledById);

            if (filters.RoomId.HasValue)
                reservationsQuery = reservationsQuery.Where(r => r.RoomId == filters.RoomId);

            if (filters.BuildingId.HasValue)
                reservationsQuery = reservationsQuery.Where(r => r.Room.BuildingId == filters.BuildingId);

            if (filters.Date.HasValue)
                reservationsQuery = reservationsQuery.Where(r => r.Date == filters.Date);

            if (filters.Status.HasValue)
                reservationsQuery = reservationsQuery.Where(r => r.Status == filters.Status);

            return await reservationsQuery
                .OrderByDescending(r => r.Date)
                .ThenByDescending(r => r.StartTime)
                .ThenBy(r => r.Id)
                .ToPagedListAsync(filters);
        }

        public async Task<IReadOnlyList<Reservation>> GetActiveFutureByRoomAsync(Guid roomId)
        {
            var today = timeProvider.WarsawToday();
            var reservations = await db.Reservations
                .Where(r => r.RoomId == roomId &&
                            r.Date >= today &&
                            (r.Status == ReservationStatus.Approved || r.Status == ReservationStatus.Pending))
                .ToListAsync();

            return reservations;
        }

        public async Task<IReadOnlyList<Reservation>> GetActiveFutureByBuildingAsync(Guid buildingId)
        {
            var today = timeProvider.WarsawToday();
            var reservations = await db.Reservations
                .Where(r => r.Room.BuildingId == buildingId &&
                            r.Date >= today &&
                            (r.Status == ReservationStatus.Approved || r.Status == ReservationStatus.Pending))
                .ToListAsync();

            return reservations;
        }

        public async Task<IReadOnlyList<Reservation>> GetActiveFutureByRoomIdsAsync(IReadOnlyList<Guid> roomIds)
        {
            var today = timeProvider.WarsawToday();
            var reservations = await db.Reservations
                .Where(r => roomIds.Contains(r.RoomId) &&
                            r.Date >= today &&
                            (r.Status == ReservationStatus.Approved || r.Status == ReservationStatus.Pending))
                .ToListAsync();

            return reservations;
        }
    }
}
