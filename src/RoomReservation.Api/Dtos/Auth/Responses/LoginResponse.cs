namespace RoomReservation.Api.Dtos.Auth.Responses
{
    public record LoginResponse(
        bool Requires2FA,
        Guid? VerificationId = null,
        string? JwtToken = null
    );
}
