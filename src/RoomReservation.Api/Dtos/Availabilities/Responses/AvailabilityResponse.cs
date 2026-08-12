namespace RoomReservation.Api.Dtos.Availabilities.Responses
{
    public record AvailabilityResponse
    (
        DayOfWeek DayOfWeek,
        TimeOnly StartTime,
        TimeOnly EndTime
    );
}
