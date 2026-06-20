using RoomReservation.Core.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<Result<string>> CreateTokenAsync(Guid userId, string? ipAddress, string? userAgent);
        Task<Result<(string jwtToken, string refreshToken)>> RotateTokenAsync(Guid userId, string refreshToken);
        Task<Result<bool>> RevokeTokenAsync(Guid userId, string refreshToken);
    }
}
