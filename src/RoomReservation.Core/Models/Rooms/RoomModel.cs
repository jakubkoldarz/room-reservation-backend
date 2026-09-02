using RoomReservation.Core.Models.Availability;

namespace RoomReservation.Core.Models.Rooms
{
    public record RoomModel(
        string Identifier,
        bool RequiresApproval,
        Guid BuildingId,
        int Floor,
        int Capacity,
        IReadOnlyList<Guid> EquipmentIds,
        IReadOnlyList<AvailabilityModel> Availabilities
    );
}
