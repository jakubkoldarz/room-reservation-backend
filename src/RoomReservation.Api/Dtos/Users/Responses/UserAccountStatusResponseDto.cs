namespace RoomReservation.Api.Dtos.Users.Responses
{
    public class UserAccountStatusResponseDto
    {
        public required bool HasProfileCompleted { get; init; }
        public required bool HasEmailVerified { get; init; }
        public required bool Has2faEnabled { get; init; }
    }
}
