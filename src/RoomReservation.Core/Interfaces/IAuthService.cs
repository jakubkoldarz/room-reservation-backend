using RoomReservation.Core.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Interfaces
{
    public interface IAuthService
    {
        Task<ResultT<(string jwtToken, string refreshToken)>> LoginAsync(string email, string password, string? ipAddress, string? userAgent);
        Task<ResultT<(string jwtToken, string refreshToken)>> RegisterAsync(string email, string password, string firstname, string lastname);
    }
}
