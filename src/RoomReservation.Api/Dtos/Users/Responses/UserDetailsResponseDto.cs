using RoomReservation.Api.Dtos.Auth.Responses;
using RoomReservation.Api.Dtos.RefreshTokens.Responses;

namespace RoomReservation.Api.Dtos.Users.Responses
{
    public record UserDetailsResponseDto
    (
        BasicUserResponseDto UserInfo,
        UserAccountStatusResponseDto AccountStatus,
        RoleWithPermissionsResponseDto RoleInfo,
        IEnumerable<RefreshTokenResponseDto> RefreshTokens
    );
}
