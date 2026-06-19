using RoomReservation.Core.Results;
using System.Net;

namespace RoomReservation.Api.Dtos
{
    public record ErrorResponse(
        string Message,
        HttpStatusCode StatusCode
    );
}
