using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Services
{
    public class UserService(IUserRepository _users) : IUserService
    {
        public async Task<ResultT<User>> GetUserDetailsAsync(Guid userId)
        {
            var user = await _users.GetUserByIdAsync(userId);
            if (user == null) return ResultT<User>.Failure("User was not found", ErrorType.NotFound);
            return ResultT<User>.Success(user);
        }

        public async Task<PagedResult<User>> GetUsersAsync(UserFilter filters)
        {
            var filteredUsers = await _users.GetFilteredAsync(filters);
            return PagedResult<User>.Success(filteredUsers.users, filteredUsers.totalCount, filters.Page, filters.PageSize);
        }
    }
}
