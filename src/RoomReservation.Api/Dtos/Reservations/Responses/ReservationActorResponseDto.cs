using RoomReservation.Api.Dtos.Users.Responses;

namespace RoomReservation.Api.Dtos.Reservations.Responses
{
    public class ReservationActorResponseDto
    {
        public required DateTime At { get; init; }
        public required BasicUserResponseDto By { get; init; }
    }
}
