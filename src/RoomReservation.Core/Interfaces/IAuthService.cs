using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Interfaces
{
    internal interface IAuthService
    {
        Task LoginAsync();
    }
}
