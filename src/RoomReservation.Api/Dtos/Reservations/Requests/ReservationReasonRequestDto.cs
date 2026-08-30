using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Reservations.Requests
{
    public record ReservationReasonRequestDto
    (
        [MaxLength(100)] string? Reason
    );
}
