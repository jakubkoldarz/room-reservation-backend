using RoomReservation.Core.Enums;

namespace RoomReservation.Core.Filters
{
    public class ReservationFilter : PagedFilter
    {
        public Guid? CreatedById { get; set; }
        public Guid? ApprovedById { get; set; }
        public Guid? CanceledById { get; set; }
        public Guid? RoomId { get; set; }
        public Guid? BuildingId { get; set; }
        public DateOnly? Date { get; set; }
        public ReservationStatus? Status { get; set; }
    }
}
