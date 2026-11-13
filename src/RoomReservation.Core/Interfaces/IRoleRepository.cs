using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Models;

namespace RoomReservation.Core.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByIdAsync(Guid roleId);
        Task<Role?> GetByNameAsync(string name);
        Task<Role?> GetDefaultRoleAsync();
        Task LoadPermissionsAsync(Role role);
        Task<PagedList<Role>> GetFilteredAsync(RoleFilter filters);
        void Add(Role role);
        void Remove(Role role);
    }
}
