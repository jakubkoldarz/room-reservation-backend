using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Equipment.Requests
{
    public record EquipmentRequest
    (
        [Required, MaxLength(50)] string Name,
        [Required, MaxLength(50)] string Icon
    );
}
