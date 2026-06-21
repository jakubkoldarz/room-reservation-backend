using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RoomReservation.Core.Results
{
    public interface IResultT<T>
    {
        T? Value { get; }
        Error? Error { get; }

        [MemberNotNullWhen(true, nameof(Value))]
        [MemberNotNullWhen(false, nameof(Error))]
        bool IsSuccess { get; }
    }
}
