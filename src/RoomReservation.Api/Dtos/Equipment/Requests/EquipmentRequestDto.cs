using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Equipment.Requests
{
    public record EquipmentRequestDto
    (
        [Required, MaxLength(50)] string Name,
        [Required, MaxLength(50)] string Icon
    );
}
