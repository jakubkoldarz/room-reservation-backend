using System.Net;

namespace RoomReservation.Api.Dtos
{
    public record ErrorResponse(
        string Message,
        HttpStatusCode StatusCode,
        IEnumerable<object>? ConflictingItems = null
    );
}
