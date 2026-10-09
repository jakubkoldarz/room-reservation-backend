using RoomReservation.Core.Providers;

namespace RoomReservation.Core.Emails
{
    public class ReservationRejectedEmail : ReservationEmail
    {
        public required string RejectedBy { get; init; }
        public required DateTime RejectedAt { get; init; }
        public required string RejectReason { get; init; }

        internal override string Subject => "Twoja rezerwacja została odrzucona";

        internal override Dictionary<string, string> GetReplacements()
        {
            var replacements = GetReservationReplacements();
            replacements[nameof(RejectedBy)] = RejectedBy;
            replacements[nameof(RejectedAt)] = TimeZoneProvider.ToWarsawTime(RejectedAt).ToString("HH:mm dd.MM.yyyy");
            replacements[nameof(RejectReason)] = RejectReason;
            return replacements;
        }
    }
}
