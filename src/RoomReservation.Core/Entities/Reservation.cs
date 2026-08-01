using RoomReservation.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Entities
{
    public class Reservation
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();

        public required Guid CreatedById { get; set; }
        public User CreatedBy { get; set; } = null!;

        public Guid? ApprovedById { get; set; }
        public User? ApprovedBy { get; set; }

        public required Guid RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public required DateTime StartTime { get; set; }
        public required DateTime EndTime { get; set; }

        public string? Purpose { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ApprovedAt { get; set; }

        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;
    }
}
