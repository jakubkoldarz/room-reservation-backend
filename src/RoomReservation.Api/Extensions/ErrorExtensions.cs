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
                ErrorType.Conflict => (error.ErrorMessage, HttpStatusCode.Conflict),
                _ => ("Internal server error", HttpStatusCode.InternalServerError)
            };

            object body;
            if(error is IConflictError conflictingError)
            {
                body = new ErrorResponse(message, statusCode, conflictingError.ConflictingItems);
            }
            else
            {
                body = new ErrorResponse(message, statusCode);
            }

            return new ObjectResult(body)
            {
                StatusCode = (int)statusCode
            };
        }
    }
}
