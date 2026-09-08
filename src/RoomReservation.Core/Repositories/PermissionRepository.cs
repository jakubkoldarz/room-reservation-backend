using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Core.Repositories
{
    public class PermissionRepository(AppDbContext _db) : IPermissionRepository
    {
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

        public async Task<(IReadOnlyList<Permission> Permissions, int TotalCount)> GetFilteredAsync(PermissionFilter filters)
        {
            var query = _db.Permissions.AsQueryable();
            if(!string.IsNullOrEmpty(filters.Name))
            {
                query = query.Where(p => EF.Functions.ILike(p.Name, $"%{filters.Name}%"));
            }
            
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<IReadOnlyList<string>> GetAllAsync()
        {
            var permissions = await _db.Permissions.Select(p => p.Name).ToListAsync();
            return permissions;
        }
    }
}
