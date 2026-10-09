using Microsoft.AspNetCore.Authorization;
using RoomReservation.Api.Authorization.Requirements;
using RoomReservation.Api.Extensions;

namespace RoomReservation.Api.Authorization.Handlers
{
    public class PermissionHandler(UserAccessProvider userAccessProvider) : AuthorizationHandler<PermissionRequirement>
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

            var access = await userAccessProvider.GetAsync(userId.Value);
            if (access is not null && access.HasPermission(requirement.Permission))
            {
                context.Succeed(requirement);
                return;
            }

            context.Fail();
        }
    }
}
