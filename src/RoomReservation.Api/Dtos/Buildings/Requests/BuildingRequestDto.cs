using RoomReservation.Api.Dtos.Availabilities.Requests;
using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Buildings.Requests
{
    public class BuildingRequestDto
    {
        [Required(AllowEmptyStrings = false)]
        [Length(1, 100)]
        public required string Name { get; init; }

        [MaxLength(20)]
        public string? Identifier { get; init; }

        [Required(AllowEmptyStrings = false)]
        [Length(1, 50)]
        public required string Street { get; init; }

        [Required(AllowEmptyStrings = false)]
        [Length(1, 50)]
        public required string City { get; init; }

        [RegularExpression(@"^[0-9]{2}-[0-9]{3}$", ErrorMessage = "Invalid postal code format. Expected format: XX-XXX")]
        public required string PostalCode { get; init; }

        [Range(0, 100)]
        public required int FloorsCount { get; init; }
        public required IReadOnlyList<AvailabilityRequestDto> Availabilities { get; init; } = [];
    }
}
