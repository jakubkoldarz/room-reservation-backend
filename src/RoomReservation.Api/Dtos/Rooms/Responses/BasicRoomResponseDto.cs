namespace RoomReservation.Api.Dtos.Rooms.Responses
{
    public record BasicRoomResponseDto
    (
        Guid Id,
        string Identifier,
        bool RequiresApproval,
        int Capacity,
        int Floor
    );
}
