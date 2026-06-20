using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Services
{
    public class RefreshTokenService(ITokenProvider _tokenProvider, IUserRepository _users, IRefreshTokenRepository _tokens) : IRefreshTokenService
    {
        public async Task<Result<string>> CreateTokenAsync(Guid userId, string? ipAddress, string? userAgent)
        {
            var refreshTokenValue = _tokenProvider.GenerateRefreshToken();

            var tokenToCreate = new RefreshToken()
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                TokenHash = BCrypt.Net.BCrypt.HashPassword(refreshTokenValue),
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            await _tokens.CreateAsync(tokenToCreate);
            return Result<string>.Success(refreshTokenValue);
        }

        public async Task<Result<bool>> RevokeTokenAsync(Guid userId, string refreshToken)
        {
            var token = await _tokens.GetTokenByHashAsync(BCrypt.Net.BCrypt.HashPassword(refreshToken));
            if (token == null)
                return Result<bool>.Failure("Token was not found", ErrorType.NotFound);

            await _tokens.RevokeAsync(token.Id);
            return Result<bool>.Success(true);
        }

        public async Task<Result<(string jwtToken, string refreshToken)>> RotateTokenAsync(Guid userId, string refreshToken)
        {
            var token = await _tokens.GetTokenByHashAsync(BCrypt.Net.BCrypt.HashPassword(refreshToken));
            var user = await _users.GetUserByIdAsync(userId);
            if (token == null || user == null)
            {
                await _tokens.RevokeAllAsync(userId);
                return Result<(string jwtToken, string refreshToken)>.Failure("Token was not found", ErrorType.Unauthorized);
            }

            var refreshTokenValue = _tokenProvider.GenerateRefreshToken();
            token.ExpiresAt = DateTime.UtcNow;
            token.TokenHash = BCrypt.Net.BCrypt.HashPassword(refreshTokenValue);

            var jwtToken = _tokenProvider.GenerateJwtToken(user);

            return Result<(string jwtToken, string refreshToken)>.Success((refreshTokenValue, jwtToken));
        }
    }
}
