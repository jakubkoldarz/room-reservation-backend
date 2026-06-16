using RoomReservation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Interfaces
{
    public interface IUserService
    {
        Task<Result<User>> GetUserDetailsAsync(Guid userId);
    }
}
