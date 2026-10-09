namespace RoomReservation.Core.Models.Availability
{
    public record AvailabilityResolution(bool IsClosed, TimeOnly? StartTime, TimeOnly? EndTime)
    {
        public bool Covers(TimeOnly start, TimeOnly end)
        {
            if (IsClosed) return false;
            return start >= StartTime!.Value && end <= EndTime!.Value;
        }
    }
}
