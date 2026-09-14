using RoomReservation.Api.Dtos.Availabilities.Requests;
using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Buildings.Requests
{
    public class BuildingRequestDto
    {
        [Required, MaxLength(100)] public string Name { get; init; } = string.Empty;
        [MaxLength(20)] public string? Identifier { get; init; }
        [Required, MaxLength(50)] public string Street { get; init; } = string.Empty;
        [Required, MaxLength(50)] public string City { get; init; } = string.Empty;
        [Required, MaxLength(50), RegularExpression("^[0-9]{2}-[0-9]{3}$", ErrorMessage = "Invalid postal code format. Expected format: XX-XXX")] public string PostalCode { get; init; } = string.Empty;
        [Required, Range(0, 100)] public int FloorsCount { get; init; }
        [Required] public IReadOnlyList<AvailabilityRequestDto> Availabilities { get; init; } = [];
    }
}
