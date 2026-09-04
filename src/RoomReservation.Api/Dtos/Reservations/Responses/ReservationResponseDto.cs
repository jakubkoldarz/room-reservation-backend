namespace RoomReservation.Api.Dtos.Reservations.Responses
{
    public record ReservationResponseDto
    (
        Guid Id,
        ReservationActorResponseDto CreatedBy,
        ReservationActorResponseDto? ApprovedBy,
        ReservationActorResponseDto? CanceledBy,
        ReservationActorResponseDto? RejectedBy,
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime,
        string? Purpose,
        string? Reason,
        string Status
    );
}
