using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Events.Requests
{
    public record EventRequestDto(
        [Required] string Name,
        [Required] IReadOnlyList<Guid> RoomIds,
        [Required] DateOnly StartDate,
        [Required] DateOnly EndDate,
        [Required] bool IsClosed,
        TimeOnly? StartTime,
        TimeOnly? EndTime
    );
}
