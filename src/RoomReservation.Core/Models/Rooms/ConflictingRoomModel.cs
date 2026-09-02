namespace RoomReservation.Core.Models.Rooms
{
    public record ConflictingRoomModel(
        Guid RoomId,
        string Identifier
    );
}
