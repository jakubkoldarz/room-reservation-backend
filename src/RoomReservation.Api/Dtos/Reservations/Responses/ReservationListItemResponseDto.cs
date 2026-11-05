using RoomReservation.Api.Dtos.Buildings.Responses;
using RoomReservation.Api.Dtos.Rooms.Responses;

namespace RoomReservation.Api.Dtos.Reservations.Responses
{
    public class ReservationListItemResponseDto
    {
        public required ReservationResponseDto ReservationInfo { get; init; }
        public required BasicRoomResponseDto RoomInfo { get; init; }
        public required BasicBuildingResponseDto BuildingInfo { get; init; }
    }
}
