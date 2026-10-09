using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Extensions;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;

namespace RoomReservation.Core.Repositories
{
    public class UserRepository(AppDbContext db, TimeProvider timeProvider) : IUserRepository
    {
        public void Add(User user)
            => db.Users.Add(user);
        public async Task<User?> GetByEmailAsync(string email)
        {
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == email);
            return user;
        }
        public async Task<User?> GetByIdAsync(Guid userId)
        {
            var now = timeProvider.UtcNow();
            var user = await db.Users
                .Include(u => u.RefreshTokens.Where(rt => rt.RevokedAt == null && rt.ExpiresAt > now))
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
            return user;
        }
        public async Task<PagedList<User>> GetFilteredAsync(UserFilter filters)
        {
            var users = db.Users.AsNoTracking();

            if (!string.IsNullOrEmpty(filters.Firstname))
                users = users.Where(u => !string.IsNullOrEmpty(u.Firstname) && EF.Functions.ILike(u.Firstname, $"%{filters.Firstname.Trim()}%"));

            if (!string.IsNullOrEmpty(filters.Lastname))
                users = users.Where(u => !string.IsNullOrEmpty(u.Lastname) && EF.Functions.ILike(u.Lastname, $"%{filters.Lastname.Trim()}%"));

            if (!string.IsNullOrEmpty(filters.Email))
                users = users.Where(u => EF.Functions.ILike(u.Email, $"%{filters.Email.Trim()}%"));

            if(filters.RoleId.HasValue)
                users = users.Where(u => u.RoleId == filters.RoleId.Value);

            return await users
                .OrderBy(u => u.Lastname)
                .ThenBy(u => u.Firstname)
                .ThenBy(u => u.Id)
                .ToPagedListAsync(filters);
        }
    }
}
