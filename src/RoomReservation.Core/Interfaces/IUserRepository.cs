using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(Guid userId);
        Task<User> CreateAsync(User user);
        Task<(IEnumerable<User> users, int totalCount)> GetFilteredAsync(UserFilter filters); 
    }
}
