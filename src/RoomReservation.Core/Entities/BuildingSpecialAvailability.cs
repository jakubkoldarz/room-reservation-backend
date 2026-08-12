namespace RoomReservation.Core.Entities
{
    public class BuildingSpecialAvailability
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public required Guid BuildingId { get; set; }
        public Building Building { get; set; } = null!;

        public required DateOnly StartDate { get; set; }
        public required DateOnly EndDate { get; set; }
        public required bool IsClosed { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
    }
}
