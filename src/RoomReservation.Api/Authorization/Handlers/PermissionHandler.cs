using Microsoft.AspNetCore.Authorization;
using RoomReservation.Api.Extensions;
using RoomReservation.Core.Authorization.Requirements;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Api.Authorization.Handlers
{
    public class PermissionHandler(IPermissionService _permissionService) : AuthorizationHandler<PermissionRequirement>
    {
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var userId = context.User.GetUserId();
            if (userId is null)
            {
                context.Fail(); 
                return;
            }

            var userHasPermission = await _permissionService.UserHasPermissionAsync(userId.Value, requirement.Permission);
            if (userHasPermission)
            {
                context.Succeed(requirement);
                return;
            }
            context.Fail();
        }
    }
}
