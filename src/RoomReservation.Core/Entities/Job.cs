using RoomReservation.Core.Enums;

namespace RoomReservation.Core.Entities
{
    public class Job
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public required JobTypes JobType { get; set; }
        public required string Payload { get; set; }
        public JobStatus Status { get; set; } = JobStatus.Pending;
        public int Attempts { get; set; } = 0;
        public int MaxAttempts { get; set; } = 3;
        public string? LastError { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required DateTime NextAttemptAt { get; set; }
        public DateTime? ProcessedAt { get; set; }
    }
}
