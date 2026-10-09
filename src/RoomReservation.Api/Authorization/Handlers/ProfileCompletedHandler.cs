using Microsoft.AspNetCore.Authorization;
using RoomReservation.Api.Authorization.Requirements;
using RoomReservation.Api.Extensions;

namespace RoomReservation.Api.Authorization.Handlers
{
    public class ProfileCompletedHandler(UserAccessProvider userAccessProvider) : AuthorizationHandler<ProfileCompletedRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ProfileCompletedRequirement requirement)
        {
            var userId = context.User.GetUserId();
            if (userId is null) return;

            var access = await userAccessProvider.GetAsync(userId.Value);
            if (access is not null && access.IsProfileComplete) context.Succeed(requirement);
        }
    }
}
