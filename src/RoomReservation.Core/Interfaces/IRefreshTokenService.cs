using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<ResultT<(string jwtToken, string refreshToken)>> RotateTokenAsync(
            string refreshToken,
            string? ipAddress = null,
            string? userAgent = null);
        string CreateToken(
            Guid userId,
            string? ipAddress = null,
            string? userAgent = null);
        Task<Result> RevokeAsync(Guid userId, string refreshToken);
        Task<Result> RevokeAsync(Guid userId, Guid refreshTokenId);
        Task<Result> RevokeAllAsync(Guid userId);
        Task<Result> DeleteExpiredAsync(Guid userId);
    }
}
