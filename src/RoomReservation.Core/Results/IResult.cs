using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Results
{
    internal interface IResult<T>
    {
        bool IsSuccess { get; init; }
        T? Value { get; init; }
        string? ErrorMessage { get; init; }
    }
}
