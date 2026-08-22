using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Reservations.Requests
{
    public record CreateReservationRequest
    (
        [Required] Guid RoomId,
        [Required] DateOnly Date,
        [Required] TimeOnly StartTime,
        [Required] TimeOnly EndTime,
        [MaxLength(100)] string? Purpose
    );
}
