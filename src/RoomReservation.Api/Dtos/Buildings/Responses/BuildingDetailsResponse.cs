using RoomReservation.Api.Dtos.Availabilities.Responses;
using RoomReservation.Api.Dtos.Rooms.Responses;

namespace RoomReservation.Api.Dtos.Buildings.Responses
{
    public record BuildingDetailsResponse
    (
        BasicBuildingResponse BuildingInfo,
        IReadOnlyList<AvailabilityResponse> Availabilities,
        IReadOnlyList<SpecialAvailabilityResponse> SpecialAvailabilities,
        IReadOnlyList<BasicRoomResponse> Rooms
    );
}
