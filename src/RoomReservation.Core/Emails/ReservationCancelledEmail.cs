using RoomReservation.Core.Providers;

namespace RoomReservation.Core.Emails
{
    public class ReservationCancelledEmail : EmailMessage
    {
        public required string Title { get; init; }
        public required string RoomName { get; init; }
        public required string BuildingName { get; init; }
        public required DateOnly Date { get; init; }
        public required TimeOnly StartTime { get; init; }
        public required TimeOnly EndTime { get; init; }
        public required string CancelledBy { get; init; }
        public required DateTime CancelledAt { get; init; }
        public required string ActionUrl { get; init; }
        public required string CancelReason { get; init; }

        internal override string Subject => "Twoja rezerwacja została anulowana";

        internal override Dictionary<string, string> GetReplacements() => new()
        {
            [nameof(Title)] = Title,
            [nameof(RoomName)] = RoomName,
            [nameof(BuildingName)] = BuildingName,
            [nameof(Date)] = Date.ToString("dd.MM.yyyy"),
            [nameof(StartTime)] = StartTime.ToString("HH:mm"),
            [nameof(EndTime)] = EndTime.ToString("HH:mm"),
            [nameof(CancelledBy)] = CancelledBy,
            [nameof(CancelledAt)] = TimeZoneProvider.ToWarsawTime(CancelledAt).ToString("HH:mm dd.MM.yyyy"),
            [nameof(ActionUrl)] = ActionUrl,
            [nameof(CancelReason)] = CancelReason,
        };
    }
}
