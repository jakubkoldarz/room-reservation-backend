using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Providers;
using RoomReservation.Core.Results.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace RoomReservation.Core.Services
{
    public class RefreshTokenService(ITokenProvider _tokenProvider, IRefreshTokenRepository _refreshTokens) : IRefreshTokenService
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

            await _refreshTokens.CreateAsync(tokenToCreate);
            return ResultT<string>.Success(tokenValue);
        }

        public async Task<Result> DeleteExpiredAsync(Guid userId)
        {
            await _refreshTokens.DeleteExpiredForUserAsync(userId);
            return Result.Success();
        }

        public async Task<Result> RevokeAllAsync(Guid userId)
        {
            await _refreshTokens.RevokeAllForUserAsync(userId);
            return Result.Success();
        }

        public async Task<Result> RevokeAsync(Guid userId, string refreshToken)
        {
            var token = await _refreshTokens.GetByHashAsync(TokenProvider.HashRefreshToken(refreshToken));
            if (token == null || token.UserId != userId)
                return Result.Failure("Refresh token was not found", ErrorType.NotFound);

            token.RevokedAt = DateTime.UtcNow;
            await _refreshTokens.UpdateAsync(token);
            
            return Result.Success();
        }

        public async Task<Result> RevokeAsync(Guid userId, Guid refreshTokenId)
        {
            var token = await _refreshTokens.GetById(refreshTokenId);
            if (token == null || token.UserId != userId)
                return Result.Failure("Refresh token was not found", ErrorType.NotFound);

            token.RevokedAt = DateTime.UtcNow;
            await _refreshTokens.UpdateAsync(token);

            return Result.Success();
        }

        public async Task<ResultT<(string jwtToken, string refreshToken)>> RotateTokenAsync(
            string refreshToken,
            string? ipAddress = null,
            string? userAgent = null)
        {
            var existingToken = await _refreshTokens.GetByHashAsync(TokenProvider.HashRefreshToken(refreshToken));
            if (existingToken == null)
                return ResultT<(string, string)>.Failure("Refresh token was not found", ErrorType.Unauthorized);

            if(existingToken.IsRevoked)
            {
                await _refreshTokens.RevokeAllForUserAsync(existingToken.UserId);
                return ResultT<(string, string)>.Failure("Refresh token reuse detected, please login again", ErrorType.Unauthorized);
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

            var createdToken = await _refreshTokens.CreateAsync(tokenToCreate);
            existingToken.RevokedAt = DateTime.UtcNow;
            existingToken.ReplacedByTokenId = createdToken.Id;
            await _refreshTokens.UpdateAsync(existingToken);

            var jwtToken = _tokenProvider.GenerateJwtToken(existingToken.User);

            return ResultT<(string, string)>.Success((jwtToken, newToken));
        }
    }
}
