namespace RoomReservation.Api.Dtos.RefreshTokens.Responses
{
    public record RefreshTokenResponse
    (
        Guid Id,
        DateTime Created,
        DateTime Expires
    );
}
