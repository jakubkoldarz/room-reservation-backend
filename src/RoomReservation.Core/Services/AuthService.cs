using Microsoft.AspNetCore.Cors.Infrastructure;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Results;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public class AuthService(
        IUserRepository _users,
        ITokenProvider _tokenProvider,
        IRefreshTokenService _refreshTokenService,
        IVerificationCodeService _verificationCodeService,
        IEmailService _emailService) : IAuthService
    {
        public async Task<Result> Disable2faAsync(Guid userId)
        {
            var user = await _users.GetByIdAsync(userId);
            if (user == null) 
                return Result.Failure("User not found", ErrorType.NotFound);

            user.Is2faEnabled = false;
            await _users.UpdateAsync(user);

            return Result.Success();
        }

        public async Task<Result> Enable2faAsync(Guid userId)
        {
            var user = await _users.GetByIdAsync(userId);
            if (user == null) return Result.Failure("User not found", ErrorType.NotFound);
            user.Is2faEnabled = true;
            await _users.UpdateAsync(user);

            return Result.Success();
        }

        public async Task<ResultT<LoginResult>> LoginAsync(
            string email,
            string password,
            string? ipAddress = null,
            string? userAgent = null)
        {
            var user = await _users.GetByEmailAsync(email);
            if (user is null)
                return new Error("Invalid credentials", ErrorType.BadRequest);

            var passwordMatch = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!passwordMatch)
                return new Error("Invalid credentials", ErrorType.BadRequest);

            if(user.Is2faEnabled)
            {
                var codeResult = await _verificationCodeService.GenerateCodeAsync(user.Id, VerificationCodeType.TwoFactorLogin);
                if(!codeResult.IsSuccess)
                    return new Error(
                        $"Verification code failed to generate: ${codeResult.Error.ErrorMessage}", 
                        ErrorType.Internal
                    );

                var sendResult = await SendVerificationCodeAsync(codeResult.Value);
                if (!sendResult.IsSuccess)
                    return new Error($"An error occurred while sending the email: {sendResult.Error}", ErrorType.Internal);

                return ResultT<LoginResult>.Success(new() 
                { 
                    Requires2FA = true,
                    VerificationId = codeResult.Value.Id
                });
            }

            var tokensResult = await IssueTokensAsync(user.Id, ipAddress, userAgent);
            if (!tokensResult.IsSuccess)
                return tokensResult.Error;

            return ResultT<LoginResult>.Success(new() 
            { 
                Requires2FA = false, 
                JwtToken = tokensResult.Value.jwtToken, 
                RefreshToken = tokensResult.Value.refreshToken 
            });
        }

        public async Task<ResultT<Guid>> RegisterAsync(string email, string password)
        {
            var user = await _users.GetByEmailAsync(email);
            if (user is not null)
                return ResultT<Guid>.Failure("Email is already taken", ErrorType.BadRequest);

            var userToCreate = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            };

            var createdUser = await _users.CreateAsync(userToCreate);

            var codeResult = await _verificationCodeService.GenerateCodeAsync(createdUser.Id, VerificationCodeType.EmailActivation);
            if (!codeResult.IsSuccess)
                return codeResult.Error;
           
            var verificationCode = codeResult.Value;
            var sendResult = await SendVerificationCodeAsync(verificationCode);

            if (!sendResult.IsSuccess)
                return new Error($"An error occurred while sending the email: {sendResult.Error}", ErrorType.Internal);


            return ResultT<Guid>.Success(verificationCode.Id);
        }

        public async Task<ResultT<Guid>> ResendEmailVerificationCodeAsync(Guid verificationId)
        {
            var oldCodeResult = await _verificationCodeService.GetByIdAsync(verificationId);
            if(!oldCodeResult.IsSuccess)
                return new Error("Invalid verification", ErrorType.BadRequest);

            var codeResult = await _verificationCodeService.GenerateCodeAsync(
                oldCodeResult.Value.UserId,
                VerificationCodeType.EmailActivation);

            if (!codeResult.IsSuccess)
                return new Error($"Verification code failed to generate: ${codeResult.Error.ErrorMessage}", ErrorType.Internal);

            var sendResult = await SendVerificationCodeAsync(codeResult.Value);
            if (!sendResult.IsSuccess)
                return new Error($"An error occurred while sending the email: {sendResult.Error}", ErrorType.Internal);

            return ResultT<Guid>.Success(codeResult.Value.Id);
        }

        public async Task<ResultT<(string jwtToken, string refreshToken)>> VerifyEmailAsync(Guid verificationId, string code)
        {
            var validationResult = await _verificationCodeService.ValidateCodeAsync(
                verificationId,
                code,
                VerificationCodeType.EmailActivation);

            if (!validationResult.IsSuccess)
                return new Error($"Verification failed: ${validationResult.Error}", ErrorType.BadRequest);

            var validatedUser = validationResult.Value.User;
            validatedUser.IsEmailVerified = true;
            await _users.UpdateAsync(validatedUser);

            var refreshTokenResult = await _refreshTokenService.CreateTokenAsync(validatedUser.Id);
            if (!refreshTokenResult.IsSuccess)
                return new Error($"Token cannot be created: {refreshTokenResult.Error}", ErrorType.Internal);

            var jwtToken = _tokenProvider.GenerateJwtToken(validatedUser);
            return ResultT<(string, string)>.Success((jwtToken, refreshTokenResult.Value));
        }

        public async Task<ResultT<(string jwtToken, string refreshToken)>> VerifyLoginCodeAsync(Guid verificationId, string code, string? ipAddress = null, string? userAgent = null)
        {
            var validationResult = await _verificationCodeService.ValidateCodeAsync(
                verificationId, 
                code,
                VerificationCodeType.TwoFactorLogin);

            if(!validationResult.IsSuccess)
                return new Error($"Verification failed: ${validationResult.Error}", ErrorType.BadRequest);

            var tokensResult = await IssueTokensAsync(validationResult.Value.UserId, ipAddress, userAgent);
            return tokensResult;
        }

        private async Task<ResultT<(string jwtToken, string refreshToken)>> IssueTokensAsync(
            Guid userId,
            string? ipAddress = null,
            string? userAgent = null)
        {
            var tokenResult = await _refreshTokenService.CreateTokenAsync(userId, ipAddress, userAgent);
            if (!tokenResult.IsSuccess)
                return tokenResult.Error;

            var user = await _users.GetByIdAsync(userId);
            if(user is null)
                return new Error("User was not found", ErrorType.NotFound);

            await _refreshTokenService.DeleteExpiredAsync(userId);
            var jwtToken = _tokenProvider.GenerateJwtToken(user);

            return ResultT<(string, string)>.Success((jwtToken, tokenResult.Value));
        }

        private async Task<Result> SendVerificationCodeAsync(VerificationCode verificationCode)
        {
            TimeSpan expirationMinutes = verificationCode.ExpiresAt - DateTime.UtcNow;

            var (subject, title, purpose) = verificationCode.Type switch
            {
                VerificationCodeType.EmailActivation => ("Aktywacja konta RoomReservation", "Potwierdzenie rejestracji", "Aby zakończyć rejestrację, potwierdź swój adres email"),
                VerificationCodeType.TwoFactorLogin => ("Kod logowania RoomReservation", "Logowanie", "Wpisz poniższy kod, aby dokończyć logowanie"),
                _ => throw new ArgumentOutOfRangeException(nameof(verificationCode.Type))
            };

            var messageResult = await _emailService.GetMessageAsync("EmailVerification", new Dictionary<string, string>
            {
                ["Title"] = title,
                ["CodePurpose"] = purpose,
                ["Code"] = verificationCode.Code,
                ["ExpirationMinutes"] = Math.Ceiling(expirationMinutes.TotalMinutes).ToString()
            });

            if (!messageResult.IsSuccess)
                return messageResult.Error;

            var sendResult = await _emailService.SendEmailAsync(new EmailMessage
            {
                To = verificationCode.User.Email,
                Subject = subject,
                HtmlMessage = messageResult.Value,
            });

            return sendResult;
        }
    }
}
