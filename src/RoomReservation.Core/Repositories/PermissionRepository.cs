using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Extensions;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;

namespace RoomReservation.Core.Repositories
{
    public class PermissionRepository(AppDbContext db) : IPermissionRepository
    {
        public async Task<UserAccessModel?> GetUserAccessAsync(Guid userId)
        {
            var access = await db.Users
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.IsProfileComplete,
                    u.Role.IsSuperAdmin,
                    Permissions = u.Role.RolePermissions.Select(rp => rp.Permission.Name).ToList()
                })
                .FirstOrDefaultAsync();

            if (access is null)
                return null;

            return new UserAccessModel(access.IsProfileComplete, access.IsSuperAdmin, access.Permissions.ToHashSet());
        }

        public async Task<IReadOnlyList<string>> GetUserPermissionsAsync(Guid userId)
        {
            return await db.Users.Where(u => u.Id == userId)
                .SelectMany(u => u.Role.RolePermissions.Select(rp => rp.Permission.Name))
                .ToListAsync();
        }

        public async Task<PagedList<Permission>> GetFilteredAsync(PermissionFilter filters)
        {
            var query = db.Permissions.AsNoTracking();
            if(!string.IsNullOrEmpty(filters.Name))
            {
                query = query.Where(p => EF.Functions.ILike(p.Name, $"%{filters.Name}%"));
            }

            return await query
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToPagedListAsync(filters);
        }

        public async Task<IReadOnlyList<string>> GetAllAsync()
        {
            var permissions = await db.Permissions.Select(p => p.Name).ToListAsync();
            return permissions;
        }
    }
}
