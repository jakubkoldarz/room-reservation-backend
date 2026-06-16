namespace RoomReservation.Api.Dtos.Users.Responses
{
    public record RefreshTokenResponse
    (
        Guid Id,
        DateTime Created,
        DateTime Expires
    );
}
