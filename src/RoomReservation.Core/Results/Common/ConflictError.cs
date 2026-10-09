using RoomReservation.Core.Enums;

namespace RoomReservation.Core.Results.Common
{
    public record ConflictError<T>(string ErrorMessage, IReadOnlyList<T> Items)
        : Error(ErrorMessage, ErrorType.Conflict), IConflictError
    {
        IEnumerable<object> IConflictError.ConflictingItems => Items.Cast<object>();
    }
}
