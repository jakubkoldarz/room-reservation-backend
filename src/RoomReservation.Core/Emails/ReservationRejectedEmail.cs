using RoomReservation.Core.Providers;

namespace RoomReservation.Core.Emails
{
    public class ReservationRejectedEmail : EmailMessage
    {
        public required string Title { get; init; }
        public required string RoomName { get; init; }
        public required string BuildingName { get; init; }
        public required DateOnly Date { get; init; }
        public required TimeOnly StartTime { get; init; }
        public required TimeOnly EndTime { get; init; }
        public required string RejectedBy { get; init; }
        public required DateTime RejectedAt { get; init; }
        public required string ActionUrl { get; init; }
        public required string RejectReason { get; init; }

        internal override string Subject => "Twoja rezerwacja została odrzucona";

        internal override Dictionary<string, string> GetReplacements() => new()
        {
            [nameof(Title)] = Title,
            [nameof(RoomName)] = RoomName,
            [nameof(BuildingName)] = BuildingName,
            [nameof(Date)] = Date.ToString("dd.MM.yyyy"),
            [nameof(StartTime)] = StartTime.ToString("HH:mm"),
            [nameof(EndTime)] = EndTime.ToString("HH:mm"),
            [nameof(RejectedBy)] = RejectedBy,
            [nameof(RejectedAt)] = TimeZoneProvider.ToWarsawTime(RejectedAt).ToString("HH:mm dd.MM.yyyy"),
            [nameof(ActionUrl)] = ActionUrl,
            [nameof(RejectReason)] = RejectReason,
        };
    }
}
