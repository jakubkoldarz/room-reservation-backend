using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Availabilities.Requests
{
    public record AvailabilityRequestDto(
        [Required] DayOfWeek DayOfWeek,
        [Required] TimeOnly StartTime,
        [Required] TimeOnly EndTime
    );
}
