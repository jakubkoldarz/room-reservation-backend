namespace RoomReservation.Core.Filters
{
    public class EventFilter : PagedFilter
    {
        public string? Name { get; set; }
        public Guid? RoomId { get; set; }
        public Guid? BuildingId { get; set; }
        public bool? IsClosed { get; set; }
        public DateOnly? DateFrom { get; set; }
        public DateOnly? DateTo { get; set; }
    }
}
