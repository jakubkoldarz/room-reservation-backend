using System.Diagnostics.CodeAnalysis;

namespace RoomReservation.Core.Results.Common
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
