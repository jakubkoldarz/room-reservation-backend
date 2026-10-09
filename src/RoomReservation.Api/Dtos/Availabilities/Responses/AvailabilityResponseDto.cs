namespace RoomReservation.Api.Dtos.Availabilities.Responses
{
    public class AvailabilityResponseDto
    {
        public required DayOfWeek DayOfWeek { get; init; }
        public required TimeOnly StartTime { get; init; }
        public required TimeOnly EndTime { get; init; }
    }
}
