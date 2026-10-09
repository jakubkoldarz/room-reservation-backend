using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Api.Extensions
{
    public static class ErrorExtensions
    {
        public static ActionResult ToActionResult(this Error error)
        {
            var statusCode = error.ErrorType switch
            {
                ErrorType.BadRequest => StatusCodes.Status400BadRequest,
                ErrorType.NotFound => StatusCodes.Status404NotFound,
                ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorType.Forbidden => StatusCodes.Status403Forbidden,
                ErrorType.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = ReasonPhrases.GetReasonPhrase(statusCode),
                Detail = statusCode == StatusCodes.Status500InternalServerError ? "Internal server error" : error.ErrorMessage
            };

            if (error is IConflictError conflictError)
                problem.Extensions["conflictingItems"] = conflictError.ConflictingItems;

            return new ObjectResult(problem) { StatusCode = statusCode };
        }

        public static ActionResult ToActionResult(this Result result, Func<ActionResult> onSuccess)
            => result.IsSuccess ? onSuccess() : result.Error.ToActionResult();

        public static ActionResult ToActionResult<T>(this ResultT<T> result, Func<T, ActionResult> onSuccess)
            => result.IsSuccess ? onSuccess(result.Value) : result.Error.ToActionResult();
    }
}
