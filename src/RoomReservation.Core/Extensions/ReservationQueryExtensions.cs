using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;

namespace RoomReservation.Core.Extensions
{
    public static class ReservationQueryExtensions
    {
        public static IQueryable<Reservation> WhereActiveNotStarted(this IQueryable<Reservation> query, DateTime warsawNow)
        {
            var today = DateOnly.FromDateTime(warsawNow);
            var currentTime = TimeOnly.FromDateTime(warsawNow);

            return query.Where(r =>
                (r.Status == ReservationStatus.Approved || r.Status == ReservationStatus.Pending) &&
                (r.Date > today || (r.Date == today && r.StartTime > currentTime)));
        }
    }
}
