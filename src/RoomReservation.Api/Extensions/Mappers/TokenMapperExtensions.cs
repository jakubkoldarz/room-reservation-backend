using RoomReservation.Api.Dtos.RefreshTokens.Responses;
using RoomReservation.Core.Entities;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class TokenMapperExtensions
    {
        public static RefreshTokenResponse ToDto(this RefreshToken refreshToken)
        {
            return new RefreshTokenResponse(
                refreshToken.Id,
                refreshToken.CreatedAt,
                refreshToken.ExpiresAt,
                refreshToken.IpAddress,
                refreshToken.UserAgent
            );
        }
    }
}
