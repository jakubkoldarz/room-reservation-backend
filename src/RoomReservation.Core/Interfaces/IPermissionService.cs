using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Models;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Interfaces
{
    public interface IPermissionService
    {
        Task<UserAccessModel?> GetUserAccessAsync(Guid userId);
        Task<ResultT<IReadOnlyList<string>>> GetUserPermissionsAsync(Guid userId);
        Task<ResultT<PagedList<Permission>>> GetAllPermissionsAsync(PermissionFilter filters);
    }
}
