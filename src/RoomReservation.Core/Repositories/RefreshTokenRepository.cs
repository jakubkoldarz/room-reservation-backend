using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Extensions;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Core.Repositories
{
    public class RefreshTokenRepository(AppDbContext db, TimeProvider timeProvider) : IRefreshTokenRepository
    {
        public void Add(RefreshToken token)
            => db.RefreshTokens.Add(token);
        public async Task DeleteExpiredForUserAsync(Guid userId)
        {
            var now = timeProvider.UtcNow();
            await db.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.ExpiresAt <= now)
                .ExecuteDeleteAsync();
        }
        public async Task DeleteExpiredOlderThanAsync(TimeSpan age)
        {
            var cutoff = timeProvider.UtcNow() - age;
            await db.RefreshTokens.Where(rt => rt.ExpiresAt < cutoff).ExecuteDeleteAsync();
        }
        public async Task<RefreshToken?> GetByHashAsync(string tokenHash)
            => await db.RefreshTokens.Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
        public async Task<RefreshToken?> GetById(Guid refreshTokenId)
            => await db.RefreshTokens.FindAsync(refreshTokenId);
        public async Task RevokeAllForUserAsync(Guid userId)
        {
            var now = timeProvider.UtcNow();
            await db.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .ExecuteUpdateAsync(x => x.SetProperty(r => r.RevokedAt, now));
        }
    }
}
