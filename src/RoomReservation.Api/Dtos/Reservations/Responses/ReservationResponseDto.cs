namespace RoomReservation.Api.Dtos.Reservations.Responses
{
    public class ReservationResponseDto
    {
        public required Guid Id { get; init; }
        public required ReservationActorResponseDto CreatedBy { get; init; }
        public ReservationActorResponseDto? ApprovedBy { get; init; }
        public ReservationActorResponseDto? CanceledBy { get; init; }
        public ReservationActorResponseDto? RejectedBy { get; init; }
        public required DateOnly Date { get; init; }
        public required TimeOnly StartTime { get; init; }
        public required TimeOnly EndTime { get; init; }
        public string? Purpose { get; init; }
        public string? Reason { get; init; }
        public string Status { get; init; } = string.Empty;
    }
}
