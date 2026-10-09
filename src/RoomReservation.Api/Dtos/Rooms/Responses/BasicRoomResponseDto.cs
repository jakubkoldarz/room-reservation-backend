namespace RoomReservation.Api.Dtos.Rooms.Responses
{
    public class BasicRoomResponseDto
    {
        public required Guid Id { get; init; }
        public required string Identifier { get; init; } 
        public required bool RequiresApproval { get; init; }
        public required int Capacity { get; init; }
        public required int Floor { get; init; }
    }
}
