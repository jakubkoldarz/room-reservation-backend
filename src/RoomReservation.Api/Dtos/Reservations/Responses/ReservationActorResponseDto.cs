using RoomReservation.Api.Dtos.Users.Responses;

namespace RoomReservation.Api.Dtos.Reservations.Responses
{
    public record ReservationActorResponseDto
    (
        DateTime At,
        BasicUserResponseDto By
    );
}
