using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Availabilities.Requests
{
    public record AvailabilityRequest
    (
        [Required] DayOfWeek DayOfWeek,
        [Required] TimeOnly StartTime,
        [Required] TimeOnly EndTime
    );
}
