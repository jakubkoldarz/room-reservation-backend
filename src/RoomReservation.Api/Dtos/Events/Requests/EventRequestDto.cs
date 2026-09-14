using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Events.Requests
{
    public class EventRequestDto
    {
        [Required, MaxLength(100)] public string Name { get; init; } = string.Empty;
        [Required] public IReadOnlyList<Guid> RoomIds { get; init; } = [];
        [Required] public DateOnly StartDate { get; init; }
        [Required] public DateOnly EndDate { get; init; }
        [Required] public bool IsClosed { get; init; }
        public TimeOnly? StartTime { get; init; }
        public TimeOnly? EndTime { get; init; }
    }
}
