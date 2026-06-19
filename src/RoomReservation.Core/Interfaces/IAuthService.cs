using RoomReservation.Core.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Interfaces
{
    public interface IAuthService
    {
        Task<Result<(string jwtToken, string refreshToken)>> LoginAsync(string email, string password);
        Task<Result<(string jwtToken, string refreshToken)>> RegisterAsync(string email, string password, string firstname, string lastname);
    }
}
