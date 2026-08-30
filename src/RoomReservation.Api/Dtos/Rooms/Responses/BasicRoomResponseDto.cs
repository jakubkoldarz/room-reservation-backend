using RoomReservation.Api.Dtos.Availabilities.Responses;
using RoomReservation.Api.Dtos.Buildings.Responses;
using RoomReservation.Api.Dtos.Equipment.Responses;

namespace RoomReservation.Api.Dtos.Rooms.Responses
{
    public record BasicRoomResponseDto
    (
        Guid Id,
        string Identifier,
        bool RequiresApproval,
        int Capacity,
        int Floor,
        BasicBuildingResponseDto BuildingInfo,
        IReadOnlyList<EquipmentResponseDto> Equipment
    );
}
