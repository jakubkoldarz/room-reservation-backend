using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Core.Repositories
{
    public class PermissionRepository(AppDbContext _db) : IPermissionRepository
    {
        public async Task<IReadOnlyList<string>> GetAllAsync()
        {
            return await _db.Permissions.Select(p => p.Name).ToListAsync();
        }

        public async Task<IReadOnlyList<string>> GetUserPermissionsAsync(Guid userId)
        {
            return await _db.Users.Where(u => u.Id == userId)
                .SelectMany(u => u.Role.RolePermissions.Select(rp => rp.Permission.Name))
                .ToListAsync();
        }

        public async Task<bool> UserHasPermissionAsync(Guid userId, string permission)
        {
            return await _db.Users.Where(u => u.Id == userId)
                .AnyAsync(u => u.Role.IsSuperAdmin || u.Role.RolePermissions.Any(rp => rp.Permission.Name == permission));
        }

    }
}
