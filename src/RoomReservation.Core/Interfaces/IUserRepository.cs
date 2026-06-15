using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Interfaces
{
    internal interface IUserRepository
    {
        Task GetUserByEmail(string email);
    }
}
