using RoomReservation.Core.Results;
using System.Security.Claims;

namespace RoomReservation.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Result<Guid> GetId(this ClaimsPrincipal claims)
        {
            var stringUUID = claims.FindFirstValue(ClaimTypes.NameIdentifier);
            var parseResult = Guid.TryParse(stringUUID, out Guid id);

            if(parseResult == false || string.IsNullOrEmpty(stringUUID)) 
                return Result<Guid>.Failure("Invalid identificator", ErrorType.Unauthorized);

            return Result<Guid>.Success(id);
        }
    }
}
