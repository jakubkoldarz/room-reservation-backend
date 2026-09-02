using RoomReservation.Core.Enums;

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
    );
}
