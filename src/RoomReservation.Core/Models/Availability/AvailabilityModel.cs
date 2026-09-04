using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Core.Models.Availability
{
    public record AvailabilityModel
    (
        [Required] DayOfWeek DayOfWeek,
        [Required] TimeOnly StartTime,
        [Required] TimeOnly EndTime
    );
}
