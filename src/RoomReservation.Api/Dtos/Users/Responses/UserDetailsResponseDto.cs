using RoomReservation.Api.Dtos.Auth.Responses;
using RoomReservation.Api.Dtos.RefreshTokens.Responses;

namespace RoomReservation.Api.Dtos.Users.Responses
{
    public class UserDetailsResponseDto
    {
        public BasicUserResponseDto UserInfo { get; init; } = null!;
        public UserAccountStatusResponseDto AccountStatus { get; init; } = null!;
        public RoleWithPermissionsResponseDto RoleInfo { get; init; } = null!;
        public IEnumerable<RefreshTokenResponseDto> RefreshTokens { get; init; } = [];
    }
}
