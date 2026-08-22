namespace RoomReservation.Api.Dtos.Users.Responses
{
    public record UserAccountStatusResponse
    (
        bool HasProfileCompleted,
        bool HasEmailVerified,
        bool Has2faEnabled
    );
}
