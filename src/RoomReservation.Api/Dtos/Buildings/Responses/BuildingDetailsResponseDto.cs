using RoomReservation.Api.Dtos.Availabilities.Responses;
using RoomReservation.Api.Dtos.Rooms.Responses;

namespace RoomReservation.Api.Dtos.Buildings.Responses
{
    public record BuildingDetailsResponseDto
    (
        BasicBuildingResponseDto BuildingInfo,
        IReadOnlyList<AvailabilityResponseDto> Availabilities,
        IReadOnlyList<BasicRoomResponseDto> Rooms
    );
}
