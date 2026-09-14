namespace RoomReservation.Api.Dtos.Users.Responses
{
    public class UserAccountStatusResponseDto
    {
        public bool HasProfileCompleted { get; init; }
        public bool HasEmailVerified { get; init; }
        public bool Has2faEnabled { get; init; }
    }
}
