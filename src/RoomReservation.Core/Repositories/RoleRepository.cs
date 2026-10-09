using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Extensions;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;

namespace RoomReservation.Core.Repositories
{
    public class RoleRepository(AppDbContext db) : IRoleRepository
    {
        public void Add(Role role)
            => db.Roles.Add(role);

        public void Remove(Role role)
            => db.Roles.Remove(role);

        public async Task<PagedList<Role>> GetFilteredAsync(RoleFilter filters)
        {
            var query = db.Roles.AsNoTracking();

            if(!string.IsNullOrEmpty(filters.Name))
                query = query.Where(r => r.Name.Contains(filters.Name));

            return await query
                .OrderBy(r => r.Name)
                .ThenBy(r => r.Id)
                .ToPagedListAsync(filters);
        }

        public async Task<Role?> GetByIdAsync(Guid roleId)
        {
            var role = await db.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == roleId);
            return role;
        }

        public async Task<Role?> GetDefaultRoleAsync()
        {
            var defaultRole = await db.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.IsDefault);
            return defaultRole;
        }
    }
}
