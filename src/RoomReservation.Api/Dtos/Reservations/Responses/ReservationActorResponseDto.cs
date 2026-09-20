using RoomReservation.Api.Dtos.Users.Responses;

namespace RoomReservation.Api.Dtos.Reservations.Responses
{
    public class ReservationActorResponseDto
    {
        public DateTime At { get; init; }
        public BasicUserResponseDto By { get; init; }
    }
}
