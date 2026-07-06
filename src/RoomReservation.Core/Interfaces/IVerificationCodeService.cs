using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Results.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Interfaces
{
    public interface IVerificationCodeService
    {
        Task<ResultT<VerificationCode>> GenerateCodeAsync(Guid userId, VerificationCodeType type);
        Task<ResultT<VerificationCode>> ValidateCodeAsync(Guid verificationId, string code, VerificationCodeType type);
        Task<ResultT<VerificationCode>> GetByIdAsync(Guid verificationId);
    }
}
