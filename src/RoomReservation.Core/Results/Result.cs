using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RoomReservation.Core.Results
{
    public class Result<T> : IResult<T>
    {
        public T? Value { get; init; }
        public Error? Error { get; init; }
        
        [MemberNotNullWhen(true, nameof(Value))]
        [MemberNotNullWhen(false, nameof(Error))]
        public bool IsSuccess { get; init; }

        public static Result<T> Success(T value) 
            => new() { Value = value, IsSuccess = true };
        public static Result<T> Failure(string errorMessage, ErrorType errorType) 
            => new() { Error = new(errorMessage, errorType), IsSuccess = false };
        public static Result<T> Failure(Error error)
            => new() { Error = error };
    }
}
