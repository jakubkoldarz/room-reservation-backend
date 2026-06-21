using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken?> GetByHashAsync(string tokenHash);
        Task<RefreshToken> CreateAsync(RefreshToken token);
        Task UpdateAsync(RefreshToken token);
        Task RevokeAllForUserAsync(Guid userId);
        Task DeleteExpiredOlderThanAsync(TimeSpan age);
        Task DeleteExpiredForUserAsync(Guid userId);
    }
}