using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid userId);
        Task<User> CreateAsync(User user);
        Task<bool> IsProfileCompletedAsync(Guid userId);
        Task<(IReadOnlyList<User> Users, int TotalCount)> GetFilteredAsync(UserFilter filters); 
        Task UpdateAsync(User user);
    }
}
