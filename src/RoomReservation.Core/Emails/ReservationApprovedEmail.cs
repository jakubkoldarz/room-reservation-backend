using RoomReservation.Core.Providers;

namespace RoomReservation.Core.Emails
{
    public class ReservationApprovedEmail : ReservationEmail
    {
        public required string ApprovedBy { get; init; }
        public required DateTime ApprovedAt { get; init; }

        internal override string Subject => "Twoja rezerwacja została zatwierdzona";

        internal override Dictionary<string, string> GetReplacements()
        {
            var replacements = GetReservationReplacements();
            replacements[nameof(ApprovedBy)] = ApprovedBy;
            replacements[nameof(ApprovedAt)] = TimeZoneProvider.ToWarsawTime(ApprovedAt).ToString("HH:mm dd.MM.yyyy");
            return replacements;
        }
    }
}
