using RoomReservation.Api.Dtos.Rooms.Responses;

namespace RoomReservation.Api.Dtos.Events.Responses
{
    public class EventResponseDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public DateOnly StartDate { get; init; }
        public DateOnly EndDate { get; init; }
        public bool IsClosed { get; init; }
        public TimeOnly? StartTime { get; init; }
        public TimeOnly? EndTime { get; init; }
        public IReadOnlyList<BasicRoomResponseDto> Rooms { get; init; } = [];

    }

}
