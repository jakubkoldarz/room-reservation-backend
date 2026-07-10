using RoomReservation.Core.Entities;
using RoomReservation.Core.Results;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Interfaces
{
    public interface IAuthService
    {
        Task<ResultT<Guid>> RegisterAsync(string email, string password);
        Task<ResultT<(string jwtToken, string refreshToken)>> VerifyEmailAsync(Guid verificationId,
                                                                               string code,
                                                                               string? ipAddress = null,
                                                                               string? userAgent = null);
        Task<ResultT<Guid>> ResendEmailVerificationCodeAsync(Guid verificationId);
        Task<ResultT<LoginResult>> LoginAsync(string email,
                                              string password,
                                              string? ipAddress = null,
                                              string? userAgent = null);
        Task<ResultT<(string jwtToken, string refreshToken)>> VerifyLoginCodeAsync(Guid verificationId,
                                                                                   string code,
                                                                                   string? ipAddress = null,
                                                                                   string? userAgent = null);
        Task<Result> Enable2faAsync(Guid userId);
        Task<Result> Disable2faAsync(Guid userId);
        Task<ResultT<VerificationCode>> IssueChangeEmailAsync(Guid userId, string newEmail);
        Task<Result> VerifyChangedEmailAsync(Guid verificationId, string code);
        Task<Result> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword);
    }
}
