namespace RoomReservation.Api.Dtos.Rooms.Responses
{
    public class BasicRoomResponseDto
    {
        public Guid Id { get; init; }
        public string Identifier { get; init; } = string.Empty;
        public bool RequiresApproval { get; init; }
        public int Capacity { get; init; }
        public int Floor { get; init; }
    }
}
