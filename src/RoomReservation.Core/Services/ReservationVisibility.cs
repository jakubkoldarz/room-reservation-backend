using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;

namespace RoomReservation.Core.Services
{
    public static class ReservationVisibility
    {
        public static IReadOnlyList<Reservation> VisibleInRoomCalendar(
            IEnumerable<Reservation> reservations,
            Guid userId,
            bool canSeeAllRequests)
        {
            var visible = new List<Reservation>();
            foreach (var reservation in reservations)
            {
                if (IsVisible(reservation, userId, canSeeAllRequests))
                    visible.Add(reservation);
            }
            return visible;
        }

        private static bool IsVisible(Reservation reservation, Guid userId, bool canSeeAllRequests)
        {
            if (canSeeAllRequests)
                return true;

            if (reservation.Status != ReservationStatus.Pending)
                return true;

            return reservation.CreatedById == userId;
        }
    }
}
