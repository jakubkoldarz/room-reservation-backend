namespace RoomReservation.Core.Entities
{
    public class Event
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();

        public required string Name { get; set; }

        public required DateOnly StartDate { get; set; }
        public required DateOnly EndDate { get; set; }

        public required bool IsClosed { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }

        public ICollection<Room> Rooms { get; set; } = [];
    }
}
