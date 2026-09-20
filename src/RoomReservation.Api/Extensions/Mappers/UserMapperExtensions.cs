using RoomReservation.Api.Dtos.Auth.Responses;
using RoomReservation.Api.Dtos.Users.Responses;
using RoomReservation.Core.Entities;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class UserMapperExtensions
    {
        public static UserAccountStatusResponseDto ToAccountStatusDto(this User user)
        {
            return new UserAccountStatusResponseDto
            {
                Has2faEnabled = user.Is2faEnabled,
                HasProfileCompleted = user.IsProfileComplete,
                HasEmailVerified = user.IsEmailVerified,
            };
              
        }

        public static BasicUserResponseDto ToBasicDto(this User user)
        {
            return new BasicUserResponseDto
            {
                Id = user.Id,
                Firstname = user.Firstname,
                Lastname = user.Lastname
            };
        }

        public static UserDetailsResponseDto ToDetailsDto(this User user, IReadOnlyList<string> permissions)
        {
            return new UserDetailsResponseDto
            {
                UserInfo= user.ToBasicDto(),
                AccountStatus= user.ToAccountStatusDto(),
                RoleInfo = new RoleWithPermissionsResponseDto { Role = user.Role.Name, Permissions = [.. permissions] },
                RefreshTokens= user.RefreshTokens.Select(rf => rf.ToDto())
            };
        }
    }
}
