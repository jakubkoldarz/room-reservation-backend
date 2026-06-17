using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Interfaces
{
    public interface IUserService
    {
        Task<Result<User>> GetUserDetailsAsync(Guid userId);
        Task<PagedResult<User>> GetUsersAsync(UserFilter filters);
    }
}
