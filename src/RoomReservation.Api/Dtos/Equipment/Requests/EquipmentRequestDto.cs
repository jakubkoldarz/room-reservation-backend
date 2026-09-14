using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Equipment.Requests
{
    public class EquipmentRequestDto
    {
        [Required, MaxLength(50)] public string Name { get; init; } = string.Empty;
        [Required, MaxLength(50)] public string Icon { get; init; } = string.Empty;
    }
}
