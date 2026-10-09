using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Models.Reservations
{
    public record ConflictingReservationModel
    (
        Guid ReservationId,
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime,
        Guid RoomId,
        string Status
    )
    {
        public static ConflictingReservationModel From(Reservation reservation)
            => new(reservation.Id, reservation.Date, reservation.StartTime, reservation.EndTime, reservation.RoomId, reservation.Status.ToString().ToUpper());
    }
}
