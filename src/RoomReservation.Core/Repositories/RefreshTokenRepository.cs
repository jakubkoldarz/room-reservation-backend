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

        public async Task<RefreshToken?> GetTokenByHashAsync(string hash)
            => await _db.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.TokenHash == hash);

        public async Task<RefreshToken?> GetTokenByIdAsync(Guid tokenId)
            => await _db.RefreshTokens.FindAsync(tokenId);

        public async Task RevokeAsync(Guid tokenId)
            => await _db.RefreshTokens
                .Where(rt => rt.Id == tokenId)
                .ExecuteUpdateAsync(x => x.SetProperty(t => t.RevokedAt, DateTime.UtcNow));

        public async Task RevokeAllAsync(Guid userId)
            => await _db.RefreshTokens
                .Where(rt => rt.UserId == userId)
                .ExecuteUpdateAsync(x => x.SetProperty(t => t.RevokedAt, DateTime.UtcNow));
    }
}
