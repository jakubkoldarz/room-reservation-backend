using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace RoomReservation.Core.Results
{
    public class Error
    {
        public string ErrorMessage { get; init; } = string.Empty;
        public ErrorType ErrorType { get; init; } = ErrorType.BadRequest;
        public Error(string errorMessage, ErrorType errorType)
        {
            ErrorMessage = errorMessage;
            ErrorType = errorType;
        }
    }

    public enum ErrorType
    {
        BadRequest,
        NotFound,
        Unauthorized,
        Forbidden
    }
}
