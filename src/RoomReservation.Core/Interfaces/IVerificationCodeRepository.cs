using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;

namespace RoomReservation.Core.Interfaces
{
    public interface IVerificationCodeRepository
    {
        Task<VerificationCode?> GetByIdAsync(Guid verificationId);
        Task<VerificationCode?> GetByUserIdAsync(Guid userId, VerificationCodeType type);
        void Add(VerificationCode code);
        Task InvalidateActiveCodesAsync(Guid userId, VerificationCodeType type);
    }
}
