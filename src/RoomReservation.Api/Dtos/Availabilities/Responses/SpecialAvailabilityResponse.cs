namespace RoomReservation.Api.Dtos.Availabilities.Responses
{
    public record SpecialAvailabilityResponse
    (
        Guid Id,
        DateOnly StartDate,
        DateOnly EndDate,
        bool IsClosed,
        TimeOnly? StartTime,
        TimeOnly? EndTime
    );
}
