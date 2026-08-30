using RoomReservation.Core.Models.Availability;

namespace RoomReservation.Core.Models
{
    public record RoomRequest(
        string Identifier,
        bool RequiresApproval,
        Guid BuildingId,
        int Floor,
        int Capacity,
        IReadOnlyList<Guid> EquipmentIds,
        IReadOnlyList<AvailabilityRequest> Availabilities
    );
}
