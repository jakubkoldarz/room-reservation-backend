namespace RoomReservation.Api.Dtos.Equipment.Responses
{
    public class EquipmentResponseDto
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public required string Icon { get; init; }
    }
}
