using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Buildings.Requests
{
    public record BuildingRequest
    (
        [Required, MaxLength(100)] string Name,
        [MaxLength(20)] string? Identifier,
        [Required, MaxLength(50)] string Street,
        [Required, MaxLength(50)] string City,
        [Required, MaxLength(50), RegularExpression("^[0-9]{2}-[0-9]{3}$", ErrorMessage = "Invalid postal code format. Expected format: XX-XXX")] string PostalCode,
        [Required, Range(0, 100)] int FloorsCount
    );
}
