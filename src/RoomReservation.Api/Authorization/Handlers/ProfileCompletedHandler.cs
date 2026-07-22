using Microsoft.AspNetCore.Authorization;
using RoomReservation.Api.Extensions;
using RoomReservation.Core.Authorization.Requirements;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Core.Authorization.Handlers
{
    public class ProfileCompletedHandler(IUserRepository _users) : AuthorizationHandler<ProfileCompletedRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ProfileCompletedRequirement requirement)
        {
            var userId = context.User.GetUserId();
            if (userId is null) return;

            var isProfileCompleted = await _users.IsProfileCompletedAsync((Guid)userId);
            if (isProfileCompleted) context.Succeed(requirement);
        }
    }
}
