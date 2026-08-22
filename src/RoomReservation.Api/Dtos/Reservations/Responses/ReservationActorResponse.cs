using RoomReservation.Api.Dtos.Users.Responses;

namespace RoomReservation.Api.Dtos.Reservations.Responses
{
    public record ReservationActorResponse
    (
        DateTime At,
        BasicUserResponse By
    );
}
