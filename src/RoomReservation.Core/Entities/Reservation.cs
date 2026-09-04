using RoomReservation.Core.Enums;

namespace RoomReservation.Core.Entities
{
    public class Reservation
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();

        public Guid? CreatedById { get; set; }
        public User? CreatedBy { get; set; } = null!;

        public Guid? ApprovedById { get; set; }
        public User? ApprovedBy { get; set; }

        public Guid? CanceledById { get; set; }
        public User? CanceledBy { get; set; }

        public Guid? RejectedById { get; set; }
        public User? RejectedBy { get; set; }

        public required Guid RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public required DateOnly Date { get; set; }
        public required TimeOnly StartTime { get; set; }
        public required TimeOnly EndTime { get; set; }

        public string? Purpose { get; set; }
        public string? Reason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }
        public DateTime? CanceledAt { get; set; }
        public DateTime? RejectedAt { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    }
}
