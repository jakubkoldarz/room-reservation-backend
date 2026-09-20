using RoomReservation.Api.Dtos.Buildings.Responses;
using RoomReservation.Api.Dtos.Equipment.Responses;

namespace RoomReservation.Api.Dtos.Rooms.Responses
{
    public class RoomResponseDto
    {
        public BasicRoomResponseDto RoomInfo { get; init; } = null!;
        public BasicBuildingResponseDto BuildingInfo { get; init; } = null!;
        public IReadOnlyList<EquipmentResponseDto> Equipment { get; init; } = [];
    }
}
