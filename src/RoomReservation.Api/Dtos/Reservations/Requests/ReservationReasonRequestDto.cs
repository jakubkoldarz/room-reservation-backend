using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Reservations.Requests
{
    public class ReservationReasonRequestDto
    {
        [MaxLength(100)] public string? Reason { get; init; }
    }
}
