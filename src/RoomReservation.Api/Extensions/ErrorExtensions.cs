using Microsoft.AspNetCore.Mvc;
using RoomReservation.Api.Dtos;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Results.Common;
using System.Net;

namespace RoomReservation.Api.Extensions
{
    public static class ErrorExtensions
    {
        public static ActionResult ToActionResult(this Error error)
        {
            var (message, statusCode) = error.ErrorType switch
            {
                ErrorType.BadRequest => (error.ErrorMessage, HttpStatusCode.BadRequest),
                ErrorType.NotFound => (error.ErrorMessage, HttpStatusCode.NotFound),
                ErrorType.Unauthorized => (error.ErrorMessage, HttpStatusCode.Unauthorized),
                ErrorType.Forbidden => (error.ErrorMessage, HttpStatusCode.Forbidden),
                _ => ("Internal server error", HttpStatusCode.InternalServerError)
            };

            return new ObjectResult(new ErrorResponse(message, statusCode))
            {
                StatusCode = (int)statusCode
            };
        }
    }
}
