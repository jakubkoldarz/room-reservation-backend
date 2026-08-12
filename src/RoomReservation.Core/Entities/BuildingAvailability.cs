namespace RoomReservation.Core.Entities
{
    public class BuildingAvailability
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public DayOfWeek DayOfWeek { get; set; }

        public Guid BuildingId { get; set; }
        public Building Building { get; set; } = null!;
    }
}
