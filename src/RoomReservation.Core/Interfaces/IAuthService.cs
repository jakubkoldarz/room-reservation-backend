using RoomReservation.Core.Entities;
using RoomReservation.Core.Results;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Interfaces
{
    public interface IAuthService
    {
        Task<ResultT<(string JwtToken, string RefreshToken)>> RegisterAsync(string email, string password, string? ipAddress = null, string? userAgent = null);
        Task<Result> ConfirmEmailAsync(Guid userId, string code);
        Task<ResultT<Guid>> IssueEmailVerificationAsync(Guid userId);
        Task<ResultT<LoginResult>> LoginAsync(string email,
                                              string password,
                                              string? ipAddress = null,
                                              string? userAgent = null);
        Task<ResultT<(string JwtToken, string RefreshToken)>> Verify2faAsync(Guid verificationId,
                                                                                   string code,
                                                                                   string? ipAddress = null,
                                                                                   string? userAgent = null);
        Task<Result> Enable2faAsync(Guid userId);
        Task<Result> Disable2faAsync(Guid userId);
        Task<ResultT<VerificationCode>> IssueChangeEmailAsync(Guid userId, string newEmail);
        Task<Result> ConfirmEmailChangeAsync(Guid verificationId, string code);
        Task<Result> ChangePasswordAsync(Guid userId, string oldPassword, string newPassword);
    }
}
