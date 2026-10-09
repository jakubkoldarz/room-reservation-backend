using RoomReservation.Core.Emails;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Extensions;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Results;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public class AuthService(
        IUserRepository userRepository,
        ITokenProvider tokenProvider,
        IRefreshTokenService refreshTokenService,
        IVerificationCodeService verificationCodeService,
        IRoleRepository roleRepository,
        IEmailQueue emailQueue,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider) : IAuthService
    {
        public async Task<Result> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user is null)
                return new Error("User not found", ErrorType.NotFound);

            var passwordMatch = BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash);
            if (!passwordMatch)
                return new Error("Invalid credentials", ErrorType.BadRequest);

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            EnqueuePasswordNotification(user);
            await unitOfWork.SaveChangesAsync();

            await refreshTokenService.RevokeAllAsync(userId);
            return Result.Success();
        }
        public async Task<Result> Disable2faAsync(Guid userId, string password)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user is null)
                return new Error("User not found", ErrorType.NotFound);

            var passwordMatch = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!passwordMatch)
                return new Error("Invalid credentials", ErrorType.BadRequest);

            user.Is2faEnabled = false;
            await unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        public async Task<Result> Enable2faAsync(Guid userId)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user is null)
                return new Error("User not found", ErrorType.NotFound);

            user.Is2faEnabled = true;
            await unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        public async Task<ResultT<VerificationCode>> IssueChangeEmailAsync(Guid userId, string newEmail)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user is null)
                return new Error("User not found", ErrorType.NotFound);

            var emailExists = await userRepository.GetByEmailAsync(newEmail);
            if (emailExists is not null)
                return new Error("Email is already taken", ErrorType.BadRequest);

            var codeResult = await verificationCodeService.GenerateCodeAsync(userId, VerificationCodeType.ChangeEmail);
            if (!codeResult.IsSuccess)
                return codeResult.Error;

            user.PendingEmail = newEmail;
            EnqueueVerificationCode(codeResult.Value, newEmail);
            await unitOfWork.SaveChangesAsync();

            return ResultT<VerificationCode>.Success(codeResult.Value);
        }
        public async Task<ResultT<LoginResult>> LoginAsync(
            string email,
            string password,
            string? ipAddress = null,
            string? userAgent = null)
        {
            var user = await userRepository.GetByEmailAsync(email);
            if (user is null)
                return new Error("Invalid credentials", ErrorType.Unauthorized);

            var passwordMatch = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            if (!passwordMatch)
                return new Error("Invalid credentials", ErrorType.Unauthorized);

            if (user.Is2faEnabled)
            {
                var codeResult = await verificationCodeService.GenerateCodeAsync(user.Id, VerificationCodeType.TwoFactorLogin);
                if (!codeResult.IsSuccess)
                    return new Error(
                        $"Verification code failed to generate: {codeResult.Error.ErrorMessage}",
                        ErrorType.Internal
                    );

                EnqueueVerificationCode(codeResult.Value, user.Email);
                await unitOfWork.SaveChangesAsync();

                return ResultT<LoginResult>.Success(new()
                {
                    Requires2FA = true,
                    VerificationId = codeResult.Value.Id
                });
            }

            var tokensResult = await IssueTokensAsync(user.Id, ipAddress, userAgent);
            if (!tokensResult.IsSuccess)
                return tokensResult.Error;

            await unitOfWork.SaveChangesAsync();

            return ResultT<LoginResult>.Success(new()
            {
                Requires2FA = false,
                JwtToken = tokensResult.Value.JwtToken,
                RefreshToken = tokensResult.Value.RefreshToken
            });
        }
        public async Task<ResultT<(string JwtToken, string RefreshToken)>> RegisterAsync(string email, string password, string? ipAddress = null, string? userAgent = null)
        {
            var user = await userRepository.GetByEmailAsync(email);
            if (user is not null)
                return new Error("Email is already taken", ErrorType.BadRequest);

            var defaultRole = await roleRepository.GetDefaultRoleAsync();
            if (defaultRole is null)
                return new Error("Unexpected error: Default role not found", ErrorType.Internal);

            var userToCreate = new User
            {
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                RoleId = defaultRole.Id,
            };
            userRepository.Add(userToCreate);

            var codeResult = await verificationCodeService.GenerateCodeAsync(userToCreate.Id, VerificationCodeType.EmailActivation);
            if (!codeResult.IsSuccess)
                return codeResult.Error;

            var refreshToken = refreshTokenService.CreateToken(userToCreate.Id, ipAddress, userAgent);
            var jwtToken = tokenProvider.GenerateJwtToken(userToCreate);

            EnqueueVerificationCode(codeResult.Value, userToCreate.Email);
            await unitOfWork.SaveChangesAsync();

            return ResultT<(string, string)>.Success((jwtToken, refreshToken));
        }
        public async Task<ResultT<Guid>> IssueEmailVerificationAsync(Guid userId)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user is null)
                return new Error("User not found", ErrorType.NotFound);

            if (user.IsEmailVerified)
                return new Error("Email is already confirmed", ErrorType.BadRequest);

            var codeResult = await verificationCodeService.GenerateCodeAsync(user.Id, VerificationCodeType.EmailActivation);

            if (!codeResult.IsSuccess)
                return codeResult.Error;

            EnqueueVerificationCode(codeResult.Value, user.Email);
            await unitOfWork.SaveChangesAsync();

            return ResultT<Guid>.Success(codeResult.Value.Id);
        }
        public async Task<Result> ConfirmEmailChangeAsync(Guid verificationId, string code)
        {
            var validationResult = await verificationCodeService.ValidateCodeAsync(
                verificationId,
                code,
                VerificationCodeType.ChangeEmail);

            if (!validationResult.IsSuccess)
                return validationResult.Error;

            var user = validationResult.Value.User;

            if (user.PendingEmail is null)
                return new Error("Pending email is null", ErrorType.BadRequest);

            var emailExists = await userRepository.GetByEmailAsync(user.PendingEmail);
            if (emailExists is not null)
                return new Error("Email is already taken", ErrorType.BadRequest);

            user.Email = user.PendingEmail;
            user.PendingEmail = null;
            await unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        public async Task<Result> ConfirmEmailAsync(Guid userId, string code)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user is null)
                return new Error("User not found", ErrorType.NotFound);

            var verificationCodeResult = await verificationCodeService.GetActiveByUserIdAsync(userId, VerificationCodeType.EmailActivation);
            if (!verificationCodeResult.IsSuccess)
                return verificationCodeResult.Error;

            var validationResult = await verificationCodeService.ValidateCodeAsync(
                verificationCodeResult.Value.Id,
                code,
                VerificationCodeType.EmailActivation);

            if (!validationResult.IsSuccess)
                return validationResult.Error;

            user.IsEmailVerified = true;
            await unitOfWork.SaveChangesAsync();

            return Result.Success();
        }
        public async Task<ResultT<(string JwtToken, string RefreshToken)>> Verify2faAsync(Guid verificationId, string code, string? ipAddress = null, string? userAgent = null)
        {
            var validationResult = await verificationCodeService.ValidateCodeAsync(
                verificationId,
                code,
                VerificationCodeType.TwoFactorLogin);

            if (!validationResult.IsSuccess)
                return new Error($"Verification failed: {validationResult.Error}", ErrorType.BadRequest);

            var tokensResult = await IssueTokensAsync(validationResult.Value.UserId, ipAddress, userAgent);
            if (!tokensResult.IsSuccess)
                return tokensResult.Error;

            await unitOfWork.SaveChangesAsync();
            return tokensResult;
        }


        private async Task<ResultT<(string JwtToken, string RefreshToken)>> IssueTokensAsync(
            Guid userId,
            string? ipAddress = null,
            string? userAgent = null)
        {
            var user = await userRepository.GetByIdAsync(userId);
            if (user is null)
                return new Error("User not found", ErrorType.NotFound);

            await refreshTokenService.DeleteExpiredAsync(userId);
            var refreshToken = refreshTokenService.CreateToken(userId, ipAddress, userAgent);
            var jwtToken = tokenProvider.GenerateJwtToken(user);

            return ResultT<(string, string)>.Success((jwtToken, refreshToken));
        }
        private void EnqueueVerificationCode(VerificationCode verificationCode, string to)
        {
            TimeSpan expirationMinutes = verificationCode.ExpiresAt - timeProvider.UtcNow();

            var (subject, title, purpose) = verificationCode.Type switch
            {
                VerificationCodeType.EmailActivation => ("Aktywacja konta RoomReservation", "Potwierdzenie rejestracji", "Aby zakończyć rejestrację, potwierdź swój adres email"),
                VerificationCodeType.TwoFactorLogin => ("Kod logowania RoomReservation", "Logowanie", "Wpisz poniższy kod, aby dokończyć logowanie"),
                VerificationCodeType.ChangeEmail => ("Zmiana adresu email", "Potwierdzenie zmiany adresu email", "Wpisz poniższy kod, aby dokończyć zmianę adresu email"),
                _ => throw new ArgumentOutOfRangeException(nameof(verificationCode.Type))
            };

            var messageToSend = new VerificationCodeEmail
            {
                To = to,
                Title = title,
                Code = verificationCode.Code,
                CodePurpose = purpose,
                ExpirationMinutes = (int)Math.Ceiling(expirationMinutes.TotalMinutes),
            };

            emailQueue.Enqueue(messageToSend);
        }
        private void EnqueuePasswordNotification(User user)
        {
            var title = "Alert bezpieczeństwa - Zmiana hasła";
            var messageToSend = new PasswordChangeEmail { Title = title, To = user.Email };
            emailQueue.Enqueue(messageToSend);
        }
    }
}
