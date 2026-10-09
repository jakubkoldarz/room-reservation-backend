using System.Security.Claims;

namespace RoomReservation.Api.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static Guid? GetUserId(this ClaimsPrincipal principal)
        {
            var userIdString = principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString))
            {
                return null;
            }

            if (!Guid.TryParse(userIdString, out var userId))
            {
                return null;
            }

            return userId;
        }
    }
}
