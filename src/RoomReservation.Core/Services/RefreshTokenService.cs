using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Extensions;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Providers;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public class RefreshTokenService(
        ITokenProvider tokenProvider,
        IRefreshTokenRepository refreshTokenRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider) : IRefreshTokenService
    {
        private static readonly TimeSpan TokenLifetime = TimeSpan.FromDays(7);

        public string CreateToken(
            Guid userId,
            string? ipAddress = null,
            string? userAgent = null)
        {
            (string tokenValue, string hash) = tokenProvider.GenerateRefreshToken();
            var now = timeProvider.UtcNow();

            var tokenToCreate = new RefreshToken()
            {
                UserId = userId,
                CreatedAt = now,
                ExpiresAt = now.Add(TokenLifetime),
                TokenHash = hash,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            refreshTokenRepository.Add(tokenToCreate);
            return tokenValue;
        }

        public async Task<Result> DeleteExpiredAsync(Guid userId)
        {
            await refreshTokenRepository.DeleteExpiredForUserAsync(userId);
            return Result.Success();
        }

        public async Task<Result> RevokeAllAsync(Guid userId)
        {
            await refreshTokenRepository.RevokeAllForUserAsync(userId);
            return Result.Success();
        }

        public async Task<Result> RevokeAsync(Guid userId, string refreshToken)
        {
            var token = await refreshTokenRepository.GetByHashAsync(TokenProvider.HashRefreshToken(refreshToken));
            if (token is null || token.UserId != userId)
                return new Error("Refresh token was not found", ErrorType.NotFound);

            token.RevokedAt = timeProvider.UtcNow();
            await unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> RevokeAsync(Guid userId, Guid refreshTokenId)
        {
            var token = await refreshTokenRepository.GetById(refreshTokenId);
            if (token is null || token.UserId != userId)
                return new Error("Refresh token was not found", ErrorType.NotFound);

            token.RevokedAt = timeProvider.UtcNow();
            await unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<ResultT<(string jwtToken, string refreshToken)>> RotateTokenAsync(
            string refreshToken,
            string? ipAddress = null,
            string? userAgent = null)
        {
            var existingToken = await refreshTokenRepository.GetByHashAsync(TokenProvider.HashRefreshToken(refreshToken));
            if (existingToken is null)
                return new Error("Refresh token was not found", ErrorType.Unauthorized);

            if (existingToken.IsRevoked)
            {
                await refreshTokenRepository.RevokeAllForUserAsync(existingToken.UserId);
                return new Error("Refresh token reuse detected, please login again", ErrorType.Unauthorized);
            }

            var now = timeProvider.UtcNow();
            if (existingToken.ExpiresAt < now)
                return new Error("Refresh token expired", ErrorType.Unauthorized);

            (var newToken, var newHash) = tokenProvider.GenerateRefreshToken();
            var tokenToCreate = new RefreshToken()
            {
                UserId = existingToken.UserId,
                CreatedAt = now,
                ExpiresAt = now.Add(TokenLifetime),
                TokenHash = newHash,
                IpAddress = ipAddress,
                UserAgent = userAgent,
            };

            refreshTokenRepository.Add(tokenToCreate);
            existingToken.RevokedAt = now;
            existingToken.ReplacedByTokenId = tokenToCreate.Id;
            await unitOfWork.SaveChangesAsync();

            var jwtToken = tokenProvider.GenerateJwtToken(existingToken.User);

            return ResultT<(string, string)>.Success((jwtToken, newToken));
        }
    }
}
