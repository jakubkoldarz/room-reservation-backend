using RoomReservation.Api.Dtos.Buildings.Responses;
using RoomReservation.Api.Dtos.Equipment.Responses;

namespace RoomReservation.Api.Dtos.Rooms.Responses
{
    public record RoomResponseDto
    (
        BasicRoomResponseDto RoomInfo,
        BasicBuildingResponseDto BuildingInfo,
        IReadOnlyList<EquipmentResponseDto> Equipment
    );
}
