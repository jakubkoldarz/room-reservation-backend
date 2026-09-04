namespace RoomReservation.Core.Entities
{
    public class Availability
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();

        public Guid? RoomId { get; set; }
        public Room? Room { get; set; }

        public Guid? BuildingId { get; set; }
        public Building? Building { get; set; }

        public required DayOfWeek DayOfWeek { get; set; }
        public required TimeOnly StartTime { get; set; }
        public required TimeOnly EndTime { get; set; }
    }
}
