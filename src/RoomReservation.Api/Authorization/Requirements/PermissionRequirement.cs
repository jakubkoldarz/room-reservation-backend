using Microsoft.AspNetCore.Authorization;

namespace RoomReservation.Api.Authorization.Requirements
{
    public class PermissionRequirement(string permission) : IAuthorizationRequirement
    {
        public string Permission { get; } = permission;
    }
}
