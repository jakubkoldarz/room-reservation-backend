namespace RoomReservation.Api.Dtos.Auth.Responses
{
    public record LoginResponseDto(
        bool Requires2FA,
        Guid? VerificationId = null,
        string? JwtToken = null
    );
}
