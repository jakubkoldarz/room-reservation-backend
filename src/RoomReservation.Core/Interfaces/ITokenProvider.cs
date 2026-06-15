using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Interfaces
{
    internal interface ITokenProvider
    {
        string GenerateRefreshToken();
        string GenerateJwtToken();
    }
}
