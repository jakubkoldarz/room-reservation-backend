using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Repositories
{
    public class UserRepository(AppDbContext _db) : IUserRepository
    {
        public async Task<User> CreateAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }
    }
}
