using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Core.Repositories
{
    public class RoleRepository(AppDbContext _db) : IRoleRepository
    {
        public void Add(Role role)
            => _db.Roles.Add(role);

        public void Remove(Role role)
            => _db.Roles.Remove(role);

        public async Task<(IReadOnlyList<Role> Roles, int TotalCount)> GetFilteredAsync(RoleFilter filters)
        {
            var query = _db.Roles.AsQueryable();

            if(!string.IsNullOrEmpty(filters.Name))
                query = query.Where(r => r.Name.Contains(filters.Name));

            var totalCount = await query.CountAsync();
            var filtered = await query.Skip((filters.Page - 1) * filters.PageSize)
                                .Take(filters.PageSize)
                                .ToListAsync();

            return (filtered, totalCount);
        }

        public async Task<Role?> GetByIdAsync(Guid roleId)
        {
            var role = await _db.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == roleId);
            return role;
        }

        public async Task<Role?> GetDefaultRoleAsync()
        {
            var defaultRole = await _db.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.IsDefault);
            return defaultRole;
        }
    }
}
