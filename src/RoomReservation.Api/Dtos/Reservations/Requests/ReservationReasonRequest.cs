using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Reservations.Requests
{
    public record ReservationReasonRequest
    (
        [MaxLength(100)] string? Reason
    );
}
