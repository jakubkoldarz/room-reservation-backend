using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Extensions;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Core.Repositories
{
    public class VerificationCodeRepository(AppDbContext db, TimeProvider timeProvider) : IVerificationCodeRepository
    {
        public void Add(VerificationCode code)
            => db.VerificationCodes.Add(code);
        public async Task<VerificationCode?> GetByIdAsync(Guid verificationId)
            => await db.VerificationCodes
            .Include(vc => vc.User)
            .FirstOrDefaultAsync(vc => vc.Id == verificationId);
        public async Task<VerificationCode?> GetByUserIdAsync(Guid userId, VerificationCodeType type)
        {
            var now = timeProvider.UtcNow();
            return await db.VerificationCodes
                .Include(vc => vc.User)
                .Where(vc => vc.UserId == userId
                          && vc.Type == type
                          && vc.IsUsed == false
                          && vc.ExpiresAt > now).FirstOrDefaultAsync();
        }
        public async Task InvalidateActiveCodesAsync(Guid userId, VerificationCodeType type)
        {
            var now = timeProvider.UtcNow();
            await db.VerificationCodes
                .Where(vc => vc.UserId == userId
                     && vc.Type == type
                     && vc.IsUsed == false
                     && vc.ExpiresAt > now)
                .ExecuteUpdateAsync(vc => vc.SetProperty(x => x.IsUsed, true));
        }
    }
}
