using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;

namespace RoomReservation.Core.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(Guid roleId);
        Task<Role?> GetDefaultRoleAsync();
        Task<(IReadOnlyList<Role> Roles, int TotalCount)> GetFilteredAsync(RoleFilter filters);
        Task UpdateAsync(Role role);
        Task AddAsync(Role role);
        Task DeleteAsync(Role role);
    }
}
