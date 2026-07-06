using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RoomReservation.Core.Results.Common
{
    public interface IResult
    {
        Error? Error { get; }

        [MemberNotNullWhen(false, nameof(Error))]
        bool IsSuccess { get; }
    }
}
