using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Models.Events
{
    public record ConflictingEventModel
    (
        Guid EventId,
        string Name,
        DateOnly StartDate,
        DateOnly EndDate
    )
    {
        public static ConflictingEventModel From(Event ev)
            => new(ev.Id, ev.Name, ev.StartDate, ev.EndDate);
    }
}
