namespace RoomReservation.Core.Filters
{
    public class RoomFilter : PagedFilter
    {
        public Guid? BuildingId { get; set; }
        public string? Identifier { get; set; }
        public int? MinCapacity { get; set; }
        public int? Floor { get; set; }

        public DayOfWeek? DayOfWeek { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }

        public IReadOnlyList<Guid>? EquipmentIds { get; set; }
    }
}
