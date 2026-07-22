using Microsoft.AspNetCore.Authorization;
using RoomReservation.Core.Authorization.Requirements;

namespace RoomReservation.Api.Authorization.Handlers
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            context.Succeed(requirement);
        }
    }
}
