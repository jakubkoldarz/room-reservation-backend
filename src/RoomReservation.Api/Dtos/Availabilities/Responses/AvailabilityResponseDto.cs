namespace RoomReservation.Api.Dtos.Availabilities.Responses
{
    public class AvailabilityResponseDto
    {
        public DayOfWeek DayOfWeek { get; init; }
        public TimeOnly StartTime { get; init; }
        public TimeOnly EndTime { get; init; }
    }
}
