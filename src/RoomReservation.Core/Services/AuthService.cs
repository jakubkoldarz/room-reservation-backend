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
        public async Task<Result> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
        {
            var user = await _users.GetByIdAsync(userId);
            if (user is null)
                return new Error("User not found", ErrorType.NotFound);

            var passwordMatch = BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash);
            if (!passwordMatch)
                return new Error("Invalid credentials", ErrorType.BadRequest);

            await _refreshTokenService.RevokeAllAsync(userId);  
            var sendResult = await SendPasswordNotification(user);
            if (!sendResult.IsSuccess)
                return sendResult.Error;

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _users.UpdateAsync(user);
            return Result.Success();
        }
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
        public async Task<ResultT<VerificationCode>> IssueChangeEmailAsync(Guid userId, string newEmail)
        {
            var user = await _users.GetByIdAsync(userId);
            if (user is null)
                return new Error("User not found", ErrorType.NotFound);

            var emailExists = await _users.GetByEmailAsync(newEmail);
            if (emailExists is not null)
                return new Error("Email is already taken", ErrorType.BadRequest);

            var codeResult = await _verificationCodeService.GenerateCodeAsync(userId, VerificationCodeType.ChangeEmail);
            if (!codeResult.IsSuccess)
                return codeResult.Error;

            user.PendingEmail = newEmail;
            await _users.UpdateAsync(user);

            var sendResult = await SendVerificationCodeAsync(codeResult.Value, user.PendingEmail);
            if (!sendResult.IsSuccess)
                return sendResult.Error;

            return ResultT<VerificationCode>.Success(codeResult.Value);
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
                JwtToken = tokensResult.Value.JwtToken, 
                RefreshToken = tokensResult.Value.RefreshToken 
            });
        }
        public async Task<ResultT<(string JwtToken, string RefreshToken)>> RegisterAsync(string email, string password, string? ipAddress = null, string? userAgent = null)
        {
            var user = await _users.GetByEmailAsync(email);
            if (user is not null)
                return new Error("Email is already taken", ErrorType.BadRequest);

            var userToCreate = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            };

            var createdUser = await _users.CreateAsync(userToCreate);

            var codeResult = await _verificationCodeService.GenerateCodeAsync(createdUser.Id, VerificationCodeType.EmailActivation);
            if (!codeResult.IsSuccess)
                return codeResult.Error;

            var refreshTokenResult = await _refreshTokenService.CreateTokenAsync(createdUser.Id, ipAddress, userAgent);
            if (!refreshTokenResult.IsSuccess)
                return refreshTokenResult.Error;

            var jwtToken = _tokenProvider.GenerateJwtToken(createdUser);

            var verificationCode = codeResult.Value;
            var sendResult = await SendVerificationCodeAsync(verificationCode);

            if (!sendResult.IsSuccess)
                return sendResult.Error;

            return ResultT<(string, string)>.Success((jwtToken, refreshTokenResult.Value));
        }
        public async Task<ResultT<Guid>> IssueEmailVerificationAsync(Guid userId)
        {
            var user = await _users.GetByIdAsync(userId);
            if (user is null)
                return new Error("User not found", ErrorType.NotFound);

            if(user.IsEmailVerified)
                return new Error("Email is already confirmed", ErrorType.BadRequest);

            var codeResult = await _verificationCodeService.GenerateCodeAsync(user.Id, VerificationCodeType.EmailActivation);

            if (!codeResult.IsSuccess)
                return codeResult.Error;

            var sendResult = await SendVerificationCodeAsync(codeResult.Value);
            if (!sendResult.IsSuccess)
                return sendResult.Error;

            return ResultT<Guid>.Success(codeResult.Value.Id);
        }
        public async Task<Result> ConfirmEmailChangeAsync(Guid verificationId, string code)
        {
            var validationResult = await _verificationCodeService.ValidateCodeAsync(
                verificationId,
                code,
                VerificationCodeType.ChangeEmail);

            if (!validationResult.IsSuccess)
                return validationResult.Error;

            var user = validationResult.Value.User;
            
            if (user.PendingEmail is null)
                return new Error("Pending email is null", ErrorType.BadRequest);

            var emailExists = await _users.GetByEmailAsync(user.PendingEmail);
            if (emailExists is not null)
                return new Error("Email is already taken", ErrorType.BadRequest);

            user.Email = user.PendingEmail;
            user.PendingEmail = null;
            await _users.UpdateAsync(user);

            return Result.Success();
        }
        public async Task<Result> ConfirmEmailAsync(Guid userId, string code)
        {
            var user = await _users.GetByIdAsync(userId);
            if (user is null)
                return new Error("User not found", ErrorType.NotFound);

            var verificationCodeResult = await _verificationCodeService.GetActiveByUserIdAsync(userId, VerificationCodeType.EmailActivation);
            if (!verificationCodeResult.IsSuccess)
                return verificationCodeResult.Error;

            var validationResult = await _verificationCodeService.ValidateCodeAsync(
                verificationCodeResult.Value.Id,
                code,
                VerificationCodeType.EmailActivation);

            if (!validationResult.IsSuccess)
                return validationResult.Error;

            user.IsEmailVerified = true;
            await _users.UpdateAsync(user);

            return Result.Success();
        }
        public async Task<ResultT<(string JwtToken, string RefreshToken)>> Verify2faAsync(Guid verificationId, string code, string? ipAddress = null, string? userAgent = null)
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
        
        
        private async Task<ResultT<(string JwtToken, string RefreshToken)>> IssueTokensAsync(
            Guid userId,
            string? ipAddress = null,
            string? userAgent = null)
        {
            var tokenResult = await _refreshTokenService.CreateTokenAsync(userId, ipAddress, userAgent);
            if (!tokenResult.IsSuccess)
                return tokenResult.Error;

            var user = await _users.GetByIdAsync(userId);
            if(user is null)
                return new Error("User not found", ErrorType.NotFound);

            await _refreshTokenService.DeleteExpiredAsync(userId);
            var jwtToken = _tokenProvider.GenerateJwtToken(user);

            return ResultT<(string, string)>.Success((jwtToken, tokenResult.Value));
        }
        private async Task<Result> SendVerificationCodeAsync(VerificationCode verificationCode, string? to = null)
        {
            TimeSpan expirationMinutes = verificationCode.ExpiresAt - DateTime.UtcNow;

            var (subject, title, purpose) = verificationCode.Type switch
            {
                VerificationCodeType.EmailActivation => ("Aktywacja konta RoomReservation", "Potwierdzenie rejestracji", "Aby zakończyć rejestrację, potwierdź swój adres email"),
                VerificationCodeType.TwoFactorLogin => ("Kod logowania RoomReservation", "Logowanie", "Wpisz poniższy kod, aby dokończyć logowanie"),
                VerificationCodeType.ChangeEmail => ("Zmiana adresu email", "Potwierdzenie zmiany adresu email", "Wpisz poniższy kod, aby dokończyć zmianę adresu email"),
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
                To = to ?? verificationCode.User.Email,
                Subject = subject,
                HtmlMessage = messageResult.Value,
            });

            return sendResult;
        }
        private async Task<Result> SendPasswordNotification(User user)
        {
            var subject = "Hasło zostało zmienione";
            var title = "Alert bezpieczeństa - Zmiana hasła";

            var messageResult = await _emailService.GetMessageAsync("PasswordChange", new Dictionary<string, string>
            {
                ["Title"] = title,
            });

            if (!messageResult.IsSuccess)
                return messageResult.Error;

            var sendResult = await _emailService.SendEmailAsync(new EmailMessage
            {
                To = user.Email,
                Subject = subject,
                HtmlMessage = messageResult.Value,
            });

            return sendResult;
        }
    }
}
