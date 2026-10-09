using RoomReservation.Api.Dtos.Availabilities.Responses;
using RoomReservation.Api.Dtos.Events.Responses;
using RoomReservation.Api.Dtos.Reservations.Responses;

namespace RoomReservation.Api.Dtos.Rooms.Responses
{
    public class RoomDetailsResponseDto
    {
        public required RoomResponseDto Details { get; init; }
        public required IReadOnlyList<ReservationResponseDto> Reservations { get; init; } = [];
        public required IReadOnlyList<AvailabilityResponseDto> Availabilities { get; init; } = [];
        public required IReadOnlyList<EventResponseDto> Events { get; init; } = [];
    }
}
