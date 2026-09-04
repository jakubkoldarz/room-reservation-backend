using RoomReservation.Api.Dtos.Availabilities.Responses;
using RoomReservation.Api.Dtos.Events.Responses;
using RoomReservation.Api.Dtos.Reservations.Responses;

namespace RoomReservation.Api.Dtos.Rooms.Responses
{
    public record RoomDetailsResponseDto
    (
        RoomResponseDto Details,
        IReadOnlyList<ReservationResponseDto> Reservations,
        IReadOnlyList<AvailabilityResponseDto> Availabilities,
        IReadOnlyList<EventResponseDto> Events
    );
}
