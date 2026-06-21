using RoomReservation.Core.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<ResultT<(string jwtToken, string refreshToken)>> RotateTokenAsync(
            string refreshToken,
            string? ipAddress = null,
            string? userAgent = null);
        Task<ResultT<string>> CreateTokenAsync(
            Guid userId,
            string? ipAddress = null,
            string? userAgent = null); 
        Task<Result> RevokeAsync(Guid userId, string refreshToken);  
        Task<Result> DeleteExpiredAsync(Guid userId);
    }
}
