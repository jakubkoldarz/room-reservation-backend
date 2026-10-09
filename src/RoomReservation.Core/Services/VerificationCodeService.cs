using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Extensions;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Results.Common;
using System.Security.Cryptography;

namespace RoomReservation.Core.Services
{
    public class VerificationCodeService(
        IVerificationCodeRepository verificationCodeRepository,
        TimeProvider timeProvider) : IVerificationCodeService
    {
        public async Task<ResultT<VerificationCode>> GenerateCodeAsync(Guid userId, VerificationCodeType type)
        {
            await verificationCodeRepository.InvalidateActiveCodesAsync(userId, type);

            var now = timeProvider.UtcNow();
            var codeToCreate = new VerificationCode()
            {
                Type = type,
                UserId = userId,
                Code = GenerateCodeValue(),
                CreatedAt = now,
                ExpiresAt = now.AddMinutes(GetExpirationMinutes(type))
            };
            verificationCodeRepository.Add(codeToCreate);

            return ResultT<VerificationCode>.Success(codeToCreate);
        }
        public async Task<ResultT<VerificationCode>> GetByIdAsync(Guid verificationId)
        {
            var verificationCode = await verificationCodeRepository.GetByIdAsync(verificationId);
            if (verificationCode is null)
                return new Error("Invalid verification id", ErrorType.NotFound);

            return ResultT<VerificationCode>.Success(verificationCode);
        }
        public async Task<ResultT<VerificationCode>> GetActiveByUserIdAsync(Guid userId, VerificationCodeType type)
        {
            var code = await verificationCodeRepository.GetByUserIdAsync(userId, type);
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
                || timeProvider.UtcNow() >= verificationCode.ExpiresAt
                || verificationCode.Code != code
                || verificationCode.Type != type)
                return new Error("Invalid code provided", ErrorType.BadRequest);

            verificationCode.IsUsed = true;
            return ResultT<VerificationCode>.Success(verificationCode);
        }


        private string GenerateCodeValue()
        {
            return RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");
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
