using RoomReservation.Core.Providers;
using System.Globalization;

namespace RoomReservation.Core.Emails
{
    public class ReservationCancelledEmail : ReservationEmail
    {
        public required string CancelledBy { get; init; }
        public required DateTime CancelledAt { get; init; }
        public required string CancelReason { get; init; }

        internal override string Subject => "Your reservation was canceled";

        internal override Dictionary<string, string> GetReplacements()
        {
            var replacements = GetReservationReplacements();
            replacements[nameof(CancelledBy)] = CancelledBy;
            replacements[nameof(CancelledAt)] = TimeZoneProvider.ToWarsawTime(CancelledAt).ToString("d MMM yyyy, HH:mm", CultureInfo.InvariantCulture);
            replacements[nameof(CancelReason)] = CancelReason;
            return replacements;
        }
    }
}
