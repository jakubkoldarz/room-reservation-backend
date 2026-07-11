using RoomReservation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Interfaces
{
    public interface ITokenProvider
    {
        (string token, string hash) GenerateRefreshToken();
        string GenerateJwtToken(User user);
    }
}
