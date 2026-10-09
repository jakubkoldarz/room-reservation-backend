using System.Net;

namespace RoomReservation.Api.Dtos
{
    public class ErrorResponse
    {
        public required string Message { get; set; }
        public required HttpStatusCode StatusCode { get; set; }
        public IEnumerable<object>? ConflictingItems { get; set; } = null;
    }

}
