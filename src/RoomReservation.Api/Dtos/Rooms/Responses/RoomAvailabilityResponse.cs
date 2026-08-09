namespace RoomReservation.Api.Dtos.Rooms.Responses
{
    public record RoomAvailabilityResponse
    (
        TimeOnly StartTime,
        TimeOnly EndTime,
        DayOfWeek DayOfWeek
    );
}
