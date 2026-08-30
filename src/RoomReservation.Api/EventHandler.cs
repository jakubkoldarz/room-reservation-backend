using Microsoft.AspNetCore.Diagnostics;
using RoomReservation.Api.Dtos;
using System.Net;

namespace RoomReservation.Api
{
    public class ExceptionHandler(ILogger<ExceptionHandler> _logger) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(exception, "An error occurred while processing the request.");

            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            httpContext.Response.ContentType = "application/json";

            var response = new ErrorResponse("Unexpected error occurred.", HttpStatusCode.InternalServerError);

            await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
            return true;
        }
    }
}
