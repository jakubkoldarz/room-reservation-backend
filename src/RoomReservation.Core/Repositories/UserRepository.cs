using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
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
        public async Task<User?> GetByEmailAsync(string email)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }
        public async Task<User?> GetByIdAsync(Guid userId)
        {
            var user = await _db.Users
                .Include(u => u.RefreshTokens.Where(rt => (rt.RevokedAt == null) && !(DateTime.UtcNow >= rt.ExpiresAt)))
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
            return user;
        }
        public async Task<(IReadOnlyList<User> Users, int TotalCount)> GetFilteredAsync(UserFilter filters)
        {
            var users = _db.Users.AsQueryable();

            if (!string.IsNullOrEmpty(filters.Firstname))
                users = users.Where(u => !string.IsNullOrEmpty(u.Firstname) && EF.Functions.ILike(u.Firstname, $"%{filters.Firstname.Trim()}%"));

            if (!string.IsNullOrEmpty(filters.Lastname)) 
                users = users.Where(u => !string.IsNullOrEmpty(u.Lastname) && EF.Functions.ILike(u.Lastname, $"%{filters.Lastname.Trim()}%"));

            if (!string.IsNullOrEmpty(filters.Email)) 
                users = users.Where(u => EF.Functions.ILike(u.Email, $"%{filters.Email.Trim()}%"));

            var totalCount = await users.CountAsync();

            var filteredUsers = await users
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            return (filteredUsers, totalCount);
        }
        public async Task<bool> IsProfileCompletedAsync(Guid userId)
        {
            return await _db.Users
                .Where(u => u.Id == userId)
                .Select(u => u.IsProfileComplete)
                .SingleOrDefaultAsync();
        }
        public async Task UpdateAsync(User user)
        {
            _db.Users.Update(user);
            await _db.SaveChangesAsync();
        }
    }
}
