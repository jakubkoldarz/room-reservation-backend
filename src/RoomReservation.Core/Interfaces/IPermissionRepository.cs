using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Models;

namespace RoomReservation.Core.Interfaces
{
    public interface IPermissionRepository
    {
        Task<UserAccessModel?> GetUserAccessAsync(Guid userId);
        Task<IReadOnlyList<string>> GetUserPermissionsAsync(Guid userId);
        Task<IReadOnlyList<string>> GetAllAsync();
        Task<PagedList<Permission>> GetFilteredAsync(PermissionFilter filters);
    }
}
