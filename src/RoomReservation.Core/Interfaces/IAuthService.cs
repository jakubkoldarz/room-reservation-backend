using RoomReservation.Core.Results;
using RoomReservation.Core.Results.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Interfaces
{
    public interface IAuthService
    {
        Task<ResultT<Guid>> RegisterAsync(string email, string password);
        Task<ResultT<(string jwtToken, string refreshToken)>> VerifyEmailAsync(Guid verificationId, string code);
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
    }
}
