using RoomReservation.Core.Entities;
using RoomReservation.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Services
{
    public class UserService(IUserRepository _users) : IUserService
    {
        public async Task<Result<User>> GetUserDetailsAsync(Guid userId)
        {
            var user = await _users.GetUserByIdAsync(userId);
            if (user == null) return Result<User>.Failure("User was not found");
            return Result<User>.Success(user);
        }
    }
}
