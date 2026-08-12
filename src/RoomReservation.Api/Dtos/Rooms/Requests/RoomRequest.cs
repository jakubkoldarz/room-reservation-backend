using RoomReservation.Api.Dtos.Availabilities.Requests;
using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Rooms.Requests
{
    public record RoomRequest
    (
        [Required, MaxLength(50)] string Identifier,
        [Required] bool RequiresApproval,
        [Required] Guid BuildingId,
        [Required] int Floor,
        [Required, Range(1, int.MaxValue, ErrorMessage = "Capacity must be a positive integer.")] int Capacity,
        [Required, MaxLength(5, ErrorMessage = "Maximum 5 equipment items allowed.")] IReadOnlyList<Guid> EquipmentIds,
        IReadOnlyList<AvailabilityRequest> Availabilities
    );
}
