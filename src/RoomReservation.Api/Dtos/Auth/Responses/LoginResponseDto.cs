namespace RoomReservation.Api.Dtos.Auth.Responses
{
    public class LoginResponseDto
    {
        public bool Requires2FA { get; init; }
        public Guid? VerificationId { get; init; }
        public string? JwtToken { get; init; }
    }
}
