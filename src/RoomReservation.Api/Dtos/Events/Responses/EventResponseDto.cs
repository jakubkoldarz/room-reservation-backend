using RoomReservation.Api.Dtos.Rooms.Responses;

namespace RoomReservation.Api.Dtos.Events.Responses
{
    public class EventResponseDto
    {
        public required Guid Id { get; init; }
        public  required string Name { get; init; } 
        public required DateOnly StartDate { get; init; }
        public required DateOnly EndDate { get; init; }
        public required bool IsClosed { get; init; }
        public TimeOnly? StartTime { get; init; }
        public TimeOnly? EndTime { get; init; }
        public required IReadOnlyList<BasicRoomResponseDto> Rooms { get; init; } = [];

    }

}
