using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;

namespace RoomReservation.Core.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(Guid roleId);
        Task<Role?> GetDefaultRoleAsync();
        Task<(IReadOnlyList<Role> Roles, int TotalCount)> GetFilteredAsync(RoleFilter filters);
        void Add(Role role);
        void Remove(Role role);
    }
}
