using RoomReservation.Api.Dtos.Availabilities.Responses;
using RoomReservation.Api.Dtos.Rooms.Responses;

namespace RoomReservation.Api.Dtos.Buildings.Responses
{
    public class BuildingDetailsResponseDto
    {
        public required BasicBuildingResponseDto BuildingInfo { get; init; } = null!;
        public required IReadOnlyList<AvailabilityResponseDto> Availabilities { get; init; } = [];
        public required IReadOnlyList<BasicRoomResponseDto> Rooms { get; init; } = [];
    }
}
