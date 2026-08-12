namespace RoomReservation.Api.Dtos.Availabilities.Requests
{
    public record SpecialAvailabilityRequest
    (
        Guid EntityId,
        DateTime StartDate,
        DateTime EndDate,
        bool IsClosed,
        TimeOnly? StartTime,
        TimeOnly? EndTime
    );
}
