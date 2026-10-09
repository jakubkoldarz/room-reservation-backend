using System.Diagnostics.CodeAnalysis;

namespace RoomReservation.Core.Results.Common
{
    public interface IResult
    {
        Error? Error { get; }

        [MemberNotNullWhen(false, nameof(Error))]
        bool IsSuccess { get; }
    }
}
