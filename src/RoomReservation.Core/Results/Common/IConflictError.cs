namespace RoomReservation.Core.Results.Common
{
    public interface IConflictError
    {
        IEnumerable<object> ConflictingItems { get; }
    }
}
