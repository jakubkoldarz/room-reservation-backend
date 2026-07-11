using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Results.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Services
{
    public class VerificationCodeService(
        IVerificationCodeRepository _verificationCodes) : IVerificationCodeService
    {
        public async Task<ResultT<VerificationCode>> GenerateCodeAsync(Guid userId, VerificationCodeType type)
        {
            await _verificationCodes.InvalidateActiveCodesAsync(userId, type);

            var codeToCreate = new VerificationCode()
            {
                Type = type,
                UserId = userId,
                Code = GenerateCodeValue(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(GetExpirationMinutes(type))
            };
            await _verificationCodes.AddAsync(codeToCreate);

            return ResultT<VerificationCode>.Success(codeToCreate);
        }
        public async Task<ResultT<VerificationCode>> GetByIdAsync(Guid verificationId)
        {
            var verificationCode = await _verificationCodes.GetByIdAsync(verificationId);
            if (verificationCode is null)
                return new Error("Invalid verification id", ErrorType.NotFound);

            return ResultT<VerificationCode>.Success(verificationCode);
        }
        public async Task<ResultT<VerificationCode>> GetActiveByUserIdAsync(Guid userId, VerificationCodeType type)
        {
            var code = await _verificationCodes.GetByUserId(userId, type);
            if (code is null)
                return new Error("Verification failed", ErrorType.BadRequest);

            return ResultT<VerificationCode>.Success(code);
        }
        public async Task<ResultT<VerificationCode>> ValidateCodeAsync(Guid verificationId, string code, VerificationCodeType type)
        {
            var codeResult = await GetByIdAsync(verificationId);
            if (!codeResult.IsSuccess)
                return codeResult.Error;

            var verificationCode = codeResult.Value;
            if (verificationCode.IsUsed
                || DateTime.UtcNow >= verificationCode.ExpiresAt
                || verificationCode.Code != code
                || verificationCode.Type != type)
                return new Error("Invalid code provided", ErrorType.BadRequest);

            return ResultT<VerificationCode>.Success(verificationCode);
        }


        private string GenerateCodeValue()
        {
            return Random.Shared.Next(100000, 999999).ToString();
        }
        private double GetExpirationMinutes(VerificationCodeType type)
        {
            return type switch
            {
                VerificationCodeType.EmailActivation => 15,
                VerificationCodeType.TwoFactorLogin => 5,
                VerificationCodeType.ChangeEmail => 10,
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }
    }
}
