using RoomReservation.Core.Providers;

namespace RoomReservation.Core.Emails
{
    public class ReservationApprovedEmail : EmailMessage
    {
        public required string Title { get; init; }
        public required string RoomName { get; init; }
        public required string BuildingName { get; init; }
        public required DateOnly Date { get; init; }
        public required TimeOnly StartTime { get; init; }
        public required TimeOnly EndTime { get; init; }
        public required string ApprovedBy { get; init; }
        public required DateTime ApprovedAt { get; init; }
        public required string ActionUrl { get; init; }

        internal override string Subject => "Twoja rezerwacja została zatwierdzona";
        internal override Dictionary<string, string> GetReplacements() => new()
        {
            [nameof(Title)] = Title,
            [nameof(RoomName)] = RoomName,
            [nameof(BuildingName)] = BuildingName,
            [nameof(Date)] = Date.ToString("dd.MM.yyyy"),
            [nameof(StartTime)] = StartTime.ToString("HH:mm"),
            [nameof(EndTime)] = EndTime.ToString("HH:mm"),
            [nameof(ApprovedBy)] = ApprovedBy,
            [nameof(ApprovedAt)] = TimeZoneProvider.ToWarsawTime(ApprovedAt).ToString("HH:mm dd.MM.yyyy"),
            [nameof(ActionUrl)] = ActionUrl,
        };
    }
    
}
