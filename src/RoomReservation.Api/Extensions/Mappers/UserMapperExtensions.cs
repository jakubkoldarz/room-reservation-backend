using RoomReservation.Api.Dtos.Auth.Responses;
using RoomReservation.Api.Dtos.Users.Responses;
using RoomReservation.Core.Entities;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class UserMapperExtensions
    {
        public static UserAccountStatusResponseDto ToAccountStatusDto(this User user)
        {
            return new UserAccountStatusResponseDto(
                user.IsProfileComplete,
                user.IsEmailVerified,
                user.Is2faEnabled
            );
        }

        public static BasicUserResponseDto ToBasicDto(this User user)
        {
            return new BasicUserResponseDto(
                user.Id,
                user.Firstname,
                user.Lastname
            );
        }

        public static UserDetailsResponseDto ToDetailsDto(this User user, IReadOnlyList<string> permissions)
        {
            return new UserDetailsResponseDto(
                user.ToBasicDto(),
                user.ToAccountStatusDto(),
                new RoleWithPermissionsResponseDto(user.Role.Name, [.. permissions]),
                user.RefreshTokens.Select(rf => rf.ToDto())
            );
        }
    }
}
