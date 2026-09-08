using RoomReservation.Core.Enums;

namespace RoomReservation.Core.Models
{
    public record JobModel(
        JobTypes JobType,
        string Payload,
        TimeSpan? Delay = null,
        int MaxAttempts = 3
    );
}
