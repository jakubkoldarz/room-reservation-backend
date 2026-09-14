using RoomReservation.Api.Dtos.Availabilities.Responses;
using RoomReservation.Api.Dtos.Rooms.Responses;

namespace RoomReservation.Api.Dtos.Buildings.Responses
{
    public class BuildingDetailsResponseDto
    {
        public BasicBuildingResponseDto BuildingInfo { get; init; } = null!;
        public IReadOnlyList<AvailabilityResponseDto> Availabilities { get; init; } = [];
        public IReadOnlyList<BasicRoomResponseDto> Rooms { get; init; } = [];
    }
}
