using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Repositories
{
    public class RefreshTokenRepository(AppDbContext _db) : IRefreshTokenRepository
    {
        public void Add(RefreshToken token)
            => _db.RefreshTokens.Add(token);
        public async Task DeleteExpiredForUserAsync(Guid userId)
        {
            await _db.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.ExpiresAt <= DateTime.UtcNow)
                .ExecuteDeleteAsync();
        }
        public async Task DeleteExpiredOlderThanAsync(TimeSpan age)
        {
            var cutoff = DateTime.UtcNow - age;
            await _db.RefreshTokens.Where(rt => rt.ExpiresAt < cutoff).ExecuteDeleteAsync();
        }
        public async Task<RefreshToken?> GetByHashAsync(string tokenHash) 
            => await _db.RefreshTokens.Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
        public async Task<RefreshToken?> GetById(Guid refreshTokenId)
            => await _db.RefreshTokens.FindAsync(refreshTokenId);
        public async Task RevokeAllForUserAsync(Guid userId)
            => await _db.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .ExecuteUpdateAsync(x => x.SetProperty(r => r.RevokedAt, DateTime.UtcNow));
    }
}
