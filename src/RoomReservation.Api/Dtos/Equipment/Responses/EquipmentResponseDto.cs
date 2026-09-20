namespace RoomReservation.Api.Dtos.Equipment.Responses
{
    public class EquipmentResponseDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string Icon { get; init; } = string.Empty;
    }
}
