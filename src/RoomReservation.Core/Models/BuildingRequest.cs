using RoomReservation.Core.Models.Availability;

namespace RoomReservation.Core.Models
{
    public record BuildingRequest(
        string Name,
        string? Identifier,
        string Street,
        string City,
        string PostalCode,
        int FloorsCount,
        IReadOnlyList<AvailabilityRequest> Availabilities
    );
}
