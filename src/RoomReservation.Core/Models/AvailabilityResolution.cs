namespace RoomReservation.Core.Services
{
    public partial class ReservationService
    {
        public record AvailabilityResolution(bool IsClosed, TimeOnly? StartTime, TimeOnly? EndTime);
    }
}
