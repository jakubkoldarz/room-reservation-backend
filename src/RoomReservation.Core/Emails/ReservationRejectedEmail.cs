using RoomReservation.Core.Providers;
using System.Globalization;

namespace RoomReservation.Core.Emails
{
    public class ReservationRejectedEmail : ReservationEmail
    {
        public required string RejectedBy { get; init; }
        public required DateTime RejectedAt { get; init; }
        public required string RejectReason { get; init; }

        internal override string Subject => "Your reservation request was rejected";

        internal override Dictionary<string, string> GetReplacements()
        {
            var replacements = GetReservationReplacements();
            replacements[nameof(RejectedBy)] = RejectedBy;
            replacements[nameof(RejectedAt)] = TimeZoneProvider.ToWarsawTime(RejectedAt).ToString("d MMM yyyy, HH:mm", CultureInfo.InvariantCulture);
            replacements[nameof(RejectReason)] = RejectReason;
            return replacements;
        }
    }
}
