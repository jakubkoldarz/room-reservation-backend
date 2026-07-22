using Microsoft.AspNetCore.Authorization;

namespace RoomReservation.Api.Attributes
{
    public class RequireCompletedProfileAttribute() : AuthorizeAttribute("ProfileCompleted");
}
