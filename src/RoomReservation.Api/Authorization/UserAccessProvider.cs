using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;

namespace RoomReservation.Api.Authorization
{
    public class UserAccessProvider(IPermissionService permissionService)
    {
        private readonly Dictionary<Guid, UserAccessModel?> _cache = [];

        public async Task<UserAccessModel?> GetAsync(Guid userId)
        {
            if (!_cache.TryGetValue(userId, out var access))
            {
                access = await permissionService.GetUserAccessAsync(userId);
                _cache[userId] = access;
            }

            return access;
        }
    }
}
