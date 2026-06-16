using RoomReservation.Core;
using System.Security.Claims;

namespace RoomReservation.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Result<Guid> GetId(this ClaimsPrincipal claims)
        {
            var stringUUID = claims.FindFirstValue(ClaimTypes.NameIdentifier);
            var parseResult = Guid.TryParse(stringUUID, out Guid id);

            if(parseResult == false) return Result<Guid>.Failure("Invalid identificator");
            return Result<Guid>.Success(id);
        }
    }
}
