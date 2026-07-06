using RoomReservation.Core.Enums;
using RoomReservation.Core.Results.Common;
using System.Security.Claims;

namespace RoomReservation.Api.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static ResultT<Guid> GetId(this ClaimsPrincipal claims)
        {
            var stringUUID = claims.FindFirstValue(ClaimTypes.NameIdentifier);
            var parseResult = Guid.TryParse(stringUUID, out Guid id);

            if(parseResult == false || string.IsNullOrEmpty(stringUUID)) 
                return ResultT<Guid>.Failure("Invalid identificator", ErrorType.Unauthorized);

            return ResultT<Guid>.Success(id);
        }
    }
}
