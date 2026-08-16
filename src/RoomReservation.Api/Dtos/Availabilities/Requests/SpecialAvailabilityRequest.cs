namespace RoomReservation.Api.Dtos.Availabilities.Requests
{
    public record SpecialAvailabilityRequest
    (
        DateOnly StartDate,
        DateOnly EndDate,
        bool IsClosed,
        TimeOnly? StartTime,
        TimeOnly? EndTime
    );
}
