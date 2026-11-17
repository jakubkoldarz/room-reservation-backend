using RoomReservation.Api.Dtos.Availabilities.Requests;
using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Rooms.Requests
{
    public class RoomRequestDto
    {
        [Required, MaxLength(50)] public string Identifier { get; init; } = string.Empty;
        [Required] public bool RequiresApproval { get; init; }
        [Required] public Guid BuildingId { get; init; }
        [Required, Range(-10, 100, ErrorMessage = "Floor must be between -10 and 100.")] public int Floor { get; init; }
        [Required, Range(1, 1000, ErrorMessage = "Capacity must be between 1 and 1000.")] public int Capacity { get; init; }
        [Required, MaxLength(5, ErrorMessage = "Maximum 5 equipment items allowed.")] public IReadOnlyList<Guid> EquipmentIds { get; init; } = [];
        [Required] public IReadOnlyList<AvailabilityRequestDto> Availabilities { get; init; } = [];
    }
}
