using RoomReservation.Api.Dtos.Availabilities.Responses;
using RoomReservation.Api.Dtos.Buildings.Responses;
using RoomReservation.Api.Dtos.Equipment.Responses;

namespace RoomReservation.Api.Dtos.Rooms.Responses
{
    public class RoomListItemResponseDto
    {
        public required BasicRoomResponseDto RoomInfo { get; init; }
        public required BasicBuildingResponseDto BuildingInfo { get; init; }
        public required IReadOnlyList<EquipmentResponseDto> Equipment { get; init; } = [];
        public required IReadOnlyList<AvailabilityResponseDto> Availabilities { get; init; } = [];
    }
}
