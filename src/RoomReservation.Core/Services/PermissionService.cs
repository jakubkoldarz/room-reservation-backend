using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Results.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Services
{
    public class PermissionService(IUserRepository _users, IPermissionRepository _permissions) : IPermissionService
    {
        async Task<ResultT<IReadOnlyList<string>>> IPermissionService.GetUserPermissionsAsync(Guid userId)
        {
            var user = await _users.GetByIdAsync(userId);
            if(user is null)
                return new Error("User not found", ErrorType.NotFound);
            
            if(user.Role.IsSuperAdmin)
            {
                var allPermissions = await _permissions.GetAllAsync();
                return ResultT<IReadOnlyList<string>>.Success(allPermissions);
            }

            var permissions = await _permissions.GetUserPermissionsAsync(userId);
            return ResultT<IReadOnlyList<string>>.Success(permissions);
        }

        async Task<bool> IPermissionService.UserHasPermissionAsync(Guid userId, string permission)
        {
           return await _permissions.UserHasPermissionAsync(userId, permission);
        }
    }
}
