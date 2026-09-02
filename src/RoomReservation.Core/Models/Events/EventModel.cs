using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Core.Models.Events
{
    public record EventModel
    (
        [Required] string Name,
        [Required] DateOnly StartDate,
        [Required] DateOnly EndDate,
        [Required] bool IsClosed,
        TimeOnly? StartTime,
        TimeOnly? EndTime
    );
}
