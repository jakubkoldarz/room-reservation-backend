using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Results.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Services
{
    public class UserService(IUserRepository _users) : IUserService
    {
        public async Task<ResultT<User>> GetUserDetailsAsync(Guid userId)
        {
            var user = await _users.GetByIdAsync(userId);
            if (user is null) 
                return new Error("User not found", ErrorType.NotFound);

            return ResultT<User>.Success(user);
        }
        public async Task<PagedResult<User>> GetUsersAsync(UserFilter filters)
        {
            var filteredUsers = await _users.GetFilteredAsync(filters);
            return PagedResult<User>.Success(filteredUsers.users, filteredUsers.totalCount, filters.Page, filters.PageSize);
        }
        public async Task<ResultT<User>> UpdateUserAsync(Guid userId, string firstname, string lastname)
        {
            var userToUpdate = await _users.GetByIdAsync(userId);
            if (userToUpdate is null)
                return new Error("User not found", ErrorType.NotFound);

            userToUpdate.Firstname = firstname;
            userToUpdate.Lastname = lastname;
            userToUpdate.IsProfileComplete = true;

            await _users.UpdateAsync(userToUpdate);
            return ResultT<User>.Success(userToUpdate);
        }
    }
}
