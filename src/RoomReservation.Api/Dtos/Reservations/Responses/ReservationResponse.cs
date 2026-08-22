namespace RoomReservation.Api.Dtos.Reservations.Responses
{
    public record ReservationResponse
    (
        Guid Id,
        ReservationActorResponse CreatedBy,
        ReservationActorResponse? ApprovedBy,
        ReservationActorResponse? CanceledBy,
        ReservationActorResponse? RejectedBy,
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime,
        string? Purpose,
        string? Reason,
        string Status
    );
}
