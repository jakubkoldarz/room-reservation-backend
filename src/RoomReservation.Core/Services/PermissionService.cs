using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public class PermissionService(IUserRepository users, IPermissionRepository permissions) : IPermissionService
    {
        public async Task<ResultT<IReadOnlyList<string>>> GetUserPermissionsAsync(Guid userId)
        {
            var user = await users.GetByIdAsync(userId);
            if (user is null)
                return new Error("User not found", ErrorType.NotFound);

            if (user.Role.IsSuperAdmin)
            {
                var allPermissions = await permissions.GetAllAsync();
                return ResultT<IReadOnlyList<string>>.Success(allPermissions);
            }

            var userPermissions = await permissions.GetUserPermissionsAsync(userId);
            return ResultT<IReadOnlyList<string>>.Success(userPermissions);
        }

        public async Task<UserAccessModel?> GetUserAccessAsync(Guid userId)
            => await permissions.GetUserAccessAsync(userId);

        public async Task<ResultT<PagedList<Permission>>> GetAllPermissionsAsync(PermissionFilter filters)
        {
            var page = await permissions.GetFilteredAsync(filters);
            return ResultT<PagedList<Permission>>.Success(page);
        }
    }
}
