using RoomReservation.Api.Dtos.Auth.Responses;
using RoomReservation.Api.Dtos.RefreshTokens.Responses;

namespace RoomReservation.Api.Dtos.Users.Responses
{
    public class UserDetailsResponseDto
    {
        public required BasicUserResponseDto UserInfo { get; init; } 
        public required UserAccountStatusResponseDto AccountStatus { get; init; }
        public required RoleWithPermissionsResponseDto RoleInfo { get; init; } 
        public required IEnumerable<RefreshTokenResponseDto> RefreshTokens { get; init; } = [];
    }
}
