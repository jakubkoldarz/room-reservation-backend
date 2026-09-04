namespace RoomReservation.Api.Dtos.Users.Responses
{
    public record UserAccountStatusResponseDto
    (
        bool HasProfileCompleted,
        bool HasEmailVerified,
        bool Has2faEnabled
    );
}
