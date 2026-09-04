namespace RoomReservation.Api.Dtos.Availabilities.Responses
{
    public record AvailabilityResponseDto
    (
        DayOfWeek DayOfWeek,
        TimeOnly StartTime,
        TimeOnly EndTime
    );
}
