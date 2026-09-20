using RoomReservation.Api.Dtos.Availabilities.Requests;
using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Rooms.Requests
{
    public class RoomRequestDto
    {
        [Required, MaxLength(50)] public string Identifier { get; init; } = string.Empty;
        [Required] public bool RequiresApproval { get; init; }
        [Required] public Guid BuildingId { get; init; }
        [Required] public int Floor { get; init; }
        [Required, Range(1, int.MaxValue, ErrorMessage = "Capacity must be a positive integer.")] public int Capacity { get; init; }
        [Required, MaxLength(5, ErrorMessage = "Maximum 5 equipment items allowed.")] public IReadOnlyList<Guid> EquipmentIds { get; init; } = [];
        [Required] public IReadOnlyList<AvailabilityRequestDto> Availabilities { get; init; } = [];
    }
}
