using RoomReservation.Core.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RoomReservation.Core.Results.Common
{
    public class Result : IResult
    {
        public Error? Error { get; init; }
        
        [MemberNotNullWhen(false, nameof(Error))]
        public bool IsSuccess { get; init; }

        public static Result Success() => new() { IsSuccess = true };
        public static Result Failure(string errorMessage, ErrorType errorType) 
            => new() { Error = new(errorMessage, errorType), IsSuccess = false };
        public static Result Failure(Error error)
            => new() { Error = error };

        public static implicit operator Result(Error error)
        {
            return Failure(error);
        }
    }
}
