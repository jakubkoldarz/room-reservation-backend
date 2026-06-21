using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Providers;
using RoomReservation.Core.Results;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace RoomReservation.Core.Services
{
    public class RefreshTokenService(ITokenProvider _tokenProvider, IRefreshTokenRepository _tokens) : IRefreshTokenService
    {
        public async Task<ResultT<string>> CreateTokenAsync(
            Guid userId,
            string? ipAddress = null,
            string? userAgent = null)
        {
            (string tokenValue, string hash) = _tokenProvider.GenerateRefreshToken();

            var tokenToCreate = new RefreshToken()
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                TokenHash = hash,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            await _tokens.CreateAsync(tokenToCreate);
            return ResultT<string>.Success(tokenValue);
        }

        public async Task<Result> DeleteExpiredAsync(Guid userId)
        {
            await _tokens.DeleteExpiredForUserAsync(userId);
            return Result.Success();
        }

        public async Task<Result> RevokeAsync(Guid userId, string refreshToken)
        {
            var token = await _tokens.GetByHashAsync(TokenProvider.HashRefreshToken(refreshToken));
            if (token == null || token.UserId != userId)
                return Result.Failure("Token was not found", ErrorType.NotFound);

            token.RevokedAt = DateTime.UtcNow;
            await _tokens.UpdateAsync(token);
            
            return Result.Success();
        }

        public async Task<ResultT<(string jwtToken, string refreshToken)>> RotateTokenAsync(
            string refreshToken,
            string? ipAddress = null,
            string? userAgent = null)
        {
            var existingToken = await _tokens.GetByHashAsync(TokenProvider.HashRefreshToken(refreshToken));
            if (existingToken == null)
                return ResultT<(string, string)>.Failure("Token was not found", ErrorType.Unauthorized);

            if(existingToken.IsRevoked)
            {
                await _tokens.RevokeAllForUserAsync(existingToken.UserId);
                return ResultT<(string, string)>.Failure("Token reuse detected, please login again", ErrorType.Unauthorized);
            }

            if (existingToken.ExpiresAt < DateTime.UtcNow)
                return ResultT<(string, string)>.Failure("Refresh token expired", ErrorType.Unauthorized);

            (var newToken, var newHash) = _tokenProvider.GenerateRefreshToken();
            var tokenToCreate = new RefreshToken()
            {
                UserId = existingToken.UserId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                TokenHash = newHash,
                IpAddress = ipAddress,
                UserAgent = userAgent,
            };

            var createdToken = await _tokens.CreateAsync(tokenToCreate);
            existingToken.RevokedAt = DateTime.UtcNow;
            existingToken.ReplacedByTokenId = createdToken.Id;
            await _tokens.UpdateAsync(existingToken);

            var jwtToken = _tokenProvider.GenerateJwtToken(existingToken.User);

            return ResultT<(string, string)>.Success((jwtToken, newToken));
        }
    }
}
