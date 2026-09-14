using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Reservations.Requests
{
    public class UpdateReservationRequestDto
    {
        [Required] public TimeOnly StartTime { get; init; }
        [Required] public TimeOnly EndTime { get; init; }
        [MaxLength(100)] public string? Purpose { get; init; }
    }
}
