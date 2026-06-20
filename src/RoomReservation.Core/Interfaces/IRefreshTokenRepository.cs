using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> CreateAsync(RefreshToken token);
        Task RevokeAsync(Guid tokenId);
        Task RevokeAllAsync(Guid userId);
        Task<RefreshToken?> GetTokenByIdAsync(Guid tokenId);
        Task<RefreshToken?> GetTokenByHashAsync(string hash);
    }
}