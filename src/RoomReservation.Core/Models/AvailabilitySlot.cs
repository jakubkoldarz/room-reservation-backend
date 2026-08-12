namespace RoomReservation.Core.Models
{
    public record AvailabilitySlot(DayOfWeek DayOfWeek, TimeOnly StartTime, TimeOnly EndTime);
}
