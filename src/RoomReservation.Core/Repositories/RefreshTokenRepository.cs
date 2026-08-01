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
        public async Task<RefreshToken> CreateAsync(RefreshToken token)
        {
            _db.RefreshTokens.Add(token);
            await _db.SaveChangesAsync();
            return token;
        }
        public async Task DeleteExpiredForUserAsync(Guid userId)
        {
            await _db.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.ExpiresAt <= DateTime.UtcNow)
                .ExecuteDeleteAsync();
        }
        public async Task DeleteExpiredOlderThanAsync(TimeSpan age)
        {
            var cutoff = DateTime.Now - age;
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
        public async Task UpdateAsync(RefreshToken token)
        {
            _db.RefreshTokens.Update(token);
            await _db.SaveChangesAsync();
        }
    }
}
