namespace RoomReservation.Api.Dtos.Users.Responses
{
    public record UserDetailsResponse
    (
        BasicUserResponse UserInfo,
        IEnumerable<RefreshTokenResponse> RefreshTokens
    );
}
