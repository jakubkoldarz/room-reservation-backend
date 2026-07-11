using RoomReservation.Api.Dtos.Users.Responses;
using RoomReservation.Core.Entities;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class UserMapperExtensions
    {
        public static BasicUserResponse ToBasicDto(this User user)
        {
            return new BasicUserResponse(
                user.Id,
                user.Firstname,
                user.Lastname,
                user.IsProfileComplete,
                user.IsEmailVerified,
                user.Is2faEnabled
            );
        }

        public static UserDetailsResponse ToDetailsDto(this User user)
        {
            return new UserDetailsResponse(
                user.ToBasicDto(),
                user.RefreshTokens.Select(rf => rf.ToDto())
            );
        }
    }
}
