using RoomReservation.Api.Dtos.Availabilities.Responses;
using RoomReservation.Api.Dtos.Reservations.Responses;

namespace RoomReservation.Api.Dtos.Rooms.Responses
{
    public record RoomDetailsResponse
    (
        BasicRoomResponse RoomInfo,
        IReadOnlyList<ReservationResponse> Reservations,
        IReadOnlyList<AvailabilityResponse> Availabilities,
        IReadOnlyList<SpecialAvailabilityResponse> SpecialAvailabilities
    );
}
