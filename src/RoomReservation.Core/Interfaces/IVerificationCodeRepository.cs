using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Interfaces
{
    public interface IVerificationCodeRepository
    {
        Task<VerificationCode?> GetByIdAsync(Guid verificationId);
        Task AddAsync(VerificationCode code);
        Task MarkAsUsedAsync(VerificationCode code);
        Task InvalidateActiveCodesAsync(Guid userId, VerificationCodeType type);
    }
}
