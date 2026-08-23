using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Core.Repositories
{
    public class ReservationRepository(AppDbContext _db) : IReservationRepository
    {
        public async Task AddAsync(Reservation reservation)
        {
            _db.Reservations.Add(reservation);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteAsync(Reservation reservation)
        {
            _db.Reservations.Remove(reservation);
            await _db.SaveChangesAsync();
        }
        public async Task<Reservation?> GetByIdAsync(Guid reservationId)
        {
            var reservation = await _db.Reservations
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
            var reservation = await _db.Reservations
                .Where(r => 
                    r.RoomId == roomId && 
                    r.Date == date && 
                    (r.Status == ReservationStatus.Approved || r.Status == ReservationStatus.Pending))
                .ToListAsync();

            return reservation;
        }

        public async Task<(IReadOnlyList<Reservation> Reservations, int TotalCount)> GetFilteredAsync(ReservationFilter filters)
        {
            var reservationsQuery = _db.Reservations
                .Include(r => r.Room)
                .Include(r => r.CreatedBy)
                .Include(r => r.ApprovedBy)
                .Include(r => r.CanceledBy)
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

            var totalCount = await reservationsQuery.CountAsync(); 
            var reservations = await reservationsQuery
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            return (reservations, totalCount);
        }

        public async Task UpdateAsync(Reservation reservation)
        {
            _db.Reservations.Update(reservation);
            await _db.SaveChangesAsync();
        }
    }
}
