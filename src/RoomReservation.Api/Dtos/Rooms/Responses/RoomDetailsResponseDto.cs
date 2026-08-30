using RoomReservation.Api.Dtos.Availabilities.Responses;
using RoomReservation.Api.Dtos.Reservations.Responses;

namespace RoomReservation.Api.Dtos.Rooms.Responses
{
    public record RoomDetailsResponseDto
    (
        BasicRoomResponseDto RoomInfo,
        IReadOnlyList<ReservationResponseDto> Reservations,
        IReadOnlyList<AvailabilityResponseDto> Availabilities
    );
}
