namespace RoomReservation.Api.Dtos.Events.Responses
{
    public record EventResponseDto(
        Guid Id,
        string Name,
        DateOnly StartDate,
        DateOnly EndDate,
        bool IsClosed,
        TimeOnly? StartTime,
        TimeOnly? EndTime,
        IReadOnlyList<Guid> RoomIds
    );
}
