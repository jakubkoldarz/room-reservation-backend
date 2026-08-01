using Microsoft.AspNetCore.Authorization;

namespace RoomReservation.Api.Attributes
{
    public class RequirePermissionAttribute(string permission) : AuthorizeAttribute($"Permission:{permission}");
}
