using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Results
{
    public class Result<T> : IResult<T>
    {
        public bool IsSuccess { get; init; }
        public T? Value { get; init; }
        public string? ErrorMessage { get; init; }

        public static Result<T> Success(T value) => new() { Value = value, IsSuccess = true };
        public static Result<T> Failure(string errorMessage) => new() { ErrorMessage = errorMessage, IsSuccess = false };
    }
}
