using RoomReservation.Api.Dtos.Auth.Responses;
using RoomReservation.Api.Dtos.RefreshTokens.Responses;

namespace RoomReservation.Api.Dtos.Users.Responses
{
    public record UserDetailsResponse
    (
        BasicUserResponse UserInfo,
        RoleWithPermissionsResponse RoleInfo,
        IEnumerable<RefreshTokenResponse> RefreshTokens
    );
}
