using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Repositories
{
    public class VerificationCodeRepository(AppDbContext _db) : IVerificationCodeRepository
    {
        public async Task AddAsync(VerificationCode code)
        {
            await _db.VerificationCodes.AddAsync(code);
            await _db.SaveChangesAsync();
        }

        public async Task<VerificationCode?> GetByIdAsync(Guid verificationId)
            => await _db.VerificationCodes
            .Include(vc => vc.User)
            .FirstOrDefaultAsync(vc => vc.Id == verificationId);

        public async Task InvalidateActiveCodesAsync(Guid userId, VerificationCodeType type)
        {
            await _db.VerificationCodes
                .Where(v => v.UserId == userId
                     && v.Type == type
                     && v.IsUsed == false
                     && v.ExpiresAt > DateTime.UtcNow)
                .ExecuteUpdateAsync(v => v.SetProperty(x => x.IsUsed, true));
        }

        public async Task MarkAsUsedAsync(VerificationCode code)
        {
            code.IsUsed = true;
            await _db.SaveChangesAsync();
        }
    }
}
