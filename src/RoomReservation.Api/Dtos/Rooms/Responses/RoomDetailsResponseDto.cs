using RoomReservation.Api.Dtos.Availabilities.Responses;
using RoomReservation.Api.Dtos.Events.Responses;
using RoomReservation.Api.Dtos.Reservations.Responses;

namespace RoomReservation.Api.Dtos.Rooms.Responses
{
    public class RoomDetailsResponseDto
    {
        public RoomResponseDto Details { get; init; } = null!;
        public IReadOnlyList<ReservationResponseDto> Reservations { get; init; } = [];
        public IReadOnlyList<AvailabilityResponseDto> Availabilities { get; init; } = [];
        public IReadOnlyList<EventResponseDto> Events { get; init; } = [];
    }
}
