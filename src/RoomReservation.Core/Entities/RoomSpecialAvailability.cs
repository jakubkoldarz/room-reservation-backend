namespace RoomReservation.Core.Entities
{
    public class RoomSpecialAvailability
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public required Guid RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public required DateOnly StartDate { get; set; }
        public required DateOnly EndDate { get; set; }
        public required bool IsClosed { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
    }
}
