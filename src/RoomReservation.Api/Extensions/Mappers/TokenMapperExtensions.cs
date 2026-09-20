using RoomReservation.Api.Dtos.RefreshTokens.Responses;
using RoomReservation.Core.Entities;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class TokenMapperExtensions
    {
        public static RefreshTokenResponseDto ToDto(this RefreshToken refreshToken)
        {
            return new RefreshTokenResponseDto
            {
                Id = refreshToken.Id,
                Created = refreshToken.CreatedAt,
                Expires = refreshToken.ExpiresAt,
                IpAddress = refreshToken.IpAddress,
                UserAgent = refreshToken.UserAgent
            };
        }
    }
}
