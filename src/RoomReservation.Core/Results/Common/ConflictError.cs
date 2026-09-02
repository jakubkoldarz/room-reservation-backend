using RoomReservation.Core.Enums;

namespace RoomReservation.Core.Results.Common
{
    public class ConflictError<T> : Error, IConflictError
    {
        public IReadOnlyList<T> Items { get; }

        IEnumerable<object> IConflictError.ConflictingItems => Items.Cast<object>();

        public ConflictError(string errorMessage, IReadOnlyList<T> items)
            : base(errorMessage, ErrorType.Conflict)
        {
            Items = items;
        }
    }
}
