using Microsoft.AspNetCore.Authorization;

namespace RoomReservation.Api.Attributes
{
    public class RequirePermissionAttribute() : AuthorizeAttribute("RequireCompletedProfile");
}
