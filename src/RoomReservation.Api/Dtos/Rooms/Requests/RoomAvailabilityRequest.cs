using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Rooms.Requests
{
    public record RoomAvailabilityRequest
    (
        [Required] DayOfWeek DayOfWeek,
        [Required] TimeOnly StartTime,
        [Required] TimeOnly EndTime
    );
}
