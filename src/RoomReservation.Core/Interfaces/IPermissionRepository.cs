using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;

namespace RoomReservation.Core.Interfaces
{
    public interface IPermissionRepository
    {
        Task<bool> UserHasPermissionAsync(Guid userId, string permission);
        Task<IReadOnlyList<string>> GetUserPermissionsAsync(Guid userId);
        Task<IReadOnlyList<string>> GetAllAsync();
        Task<(IReadOnlyList<Permission> Permissions, int TotalCount)> GetFilteredAsync(PermissionFilter filters);
    }
}
