using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Interfaces
{
    internal interface IAuthService
    {
        Task<Result<string>> LoginAsync(string email, string password);
        Task<Result<string>> RegisterAsync(string email, string password, string firstname, string lastname);
    }
}
