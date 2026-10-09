namespace RoomReservation.Core.Emails
{
    public abstract class ReservationEmail : EmailMessage
    {
        public required string Title { get; init; }
        public required string RoomName { get; init; }
        public required string BuildingName { get; init; }
        public required DateOnly Date { get; init; }
        public required TimeOnly StartTime { get; init; }
        public required TimeOnly EndTime { get; init; }
        public required string ActionUrl { get; init; }

        protected Dictionary<string, string> GetReservationReplacements() => new()
        {
            [nameof(Title)] = Title,
            [nameof(RoomName)] = RoomName,
            [nameof(BuildingName)] = BuildingName,
            [nameof(Date)] = Date.ToString("dd.MM.yyyy"),
            [nameof(StartTime)] = StartTime.ToString("HH:mm"),
            [nameof(EndTime)] = EndTime.ToString("HH:mm"),
            [nameof(ActionUrl)] = ActionUrl,
        };
    }
}
