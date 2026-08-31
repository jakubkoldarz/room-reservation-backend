using RoomReservation.Core.Models.Availability;

namespace RoomReservation.Core.Models
{
    public record BuildingModel(
        string Name,
        string? Identifier,
        string Street,
        string City,
        string PostalCode,
        int FloorsCount,
        IReadOnlyList<AvailabilityModel> Availabilities
    );
}
