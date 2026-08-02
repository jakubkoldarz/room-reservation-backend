using RoomReservation.Api.Dtos.Rooms.Responses;

namespace RoomReservation.Api.Dtos.Buildings.Responses
{
    public record BuildingDetailsResponse
    (
        BasicBuildingResponse BuildingInfo,
        IEnumerable<BasicRoomResponse> Rooms
    );
}
