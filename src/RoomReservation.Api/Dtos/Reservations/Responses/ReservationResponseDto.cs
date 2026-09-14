namespace RoomReservation.Api.Dtos.Reservations.Responses
{
    public class ReservationResponseDto
    {
        public Guid Id { get; init; }
        public ReservationActorResponseDto CreatedBy { get; init; }
        public ReservationActorResponseDto? ApprovedBy { get; init; }
        public ReservationActorResponseDto? CanceledBy { get; init; }
        public ReservationActorResponseDto? RejectedBy { get; init; }
        public DateOnly Date { get; init; }
        public TimeOnly StartTime { get; init; }
        public TimeOnly EndTime { get; init; }
        public string? Purpose { get; init; }
        public string? Reason { get; init; }
        public string Status { get; init; } = string.Empty;
    }
}
