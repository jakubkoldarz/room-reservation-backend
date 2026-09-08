using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Interfaces
{
    public interface IPermissionService
    {
        Task<bool> UserHasPermissionAsync(Guid userId, string permission);
        Task<ResultT<IReadOnlyList<string>>> GetUserPermissionsAsync(Guid userId);
        Task<PagedResult<Permission>> GetAllPermissionsAsync(PermissionFilter filters);
    }
}
