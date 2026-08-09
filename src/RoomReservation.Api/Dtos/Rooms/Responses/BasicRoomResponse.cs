using RoomReservation.Api.Dtos.Buildings.Responses;
using RoomReservation.Api.Dtos.Equipment.Responses;

namespace RoomReservation.Api.Dtos.Rooms.Responses
{
    public record BasicRoomResponse
    (
        Guid Id,
        string Identifier,
        bool RequiresApproval,
        int Capacity,
        int Floor,
        BasicBuildingResponse BuildingInfo,
        IReadOnlyList<BasicEquipmentResponse> Equipment,
        IReadOnlyList<RoomAvailabilityResponse> Availability
    );
}
