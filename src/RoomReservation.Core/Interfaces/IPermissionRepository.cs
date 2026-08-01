using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Interfaces
{
    public interface IPermissionRepository
    {
        Task<bool> UserHasPermissionAsync(Guid userId, string permission);
        Task<IReadOnlyList<string>> GetUserPermissionsAsync(Guid userId);
        Task<IReadOnlyList<string>> GetAllAsync();
    }
}
