using RoomReservation.Api.Dtos.RefreshTokens.Responses;
using RoomReservation.Core.Entities;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class TokenMapperExtensions
    {
        public static RefreshTokenResponseDto ToDto(this RefreshToken refreshToken)
        {
            return new RefreshTokenResponseDto(
                refreshToken.Id,
                refreshToken.CreatedAt,
                refreshToken.ExpiresAt,
                refreshToken.IpAddress,
                refreshToken.UserAgent
            );
        }
    }
}
