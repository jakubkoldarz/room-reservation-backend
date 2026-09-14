using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Availabilities.Requests
{
    public class AvailabilityRequestDto
    {
        [Required] public required DayOfWeek DayOfWeek { get; init; }
        [Required] public required TimeOnly StartTime { get; init; }
        [Required] public required TimeOnly EndTime { get; init; }
    }
}
    