namespace RoomReservation.Core.Models.Availability
{
    public record AvailabilityResolution(bool IsClosed, TimeOnly? StartTime, TimeOnly? EndTime);
}
