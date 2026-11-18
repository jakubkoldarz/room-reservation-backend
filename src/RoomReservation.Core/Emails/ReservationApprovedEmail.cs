using RoomReservation.Core.Providers;
using System.Globalization;

namespace RoomReservation.Core.Emails
{
    public class ReservationApprovedEmail : ReservationEmail
    {
        public required string ApprovedBy { get; init; }
        public required DateTime ApprovedAt { get; init; }

        internal override string Subject => "Your reservation was confirmed";

        internal override Dictionary<string, string> GetReplacements()
        {
            var replacements = GetReservationReplacements();
            replacements[nameof(ApprovedBy)] = ApprovedBy;
            replacements[nameof(ApprovedAt)] = TimeZoneProvider.ToWarsawTime(ApprovedAt).ToString("d MMM yyyy, HH:mm", CultureInfo.InvariantCulture);
            return replacements;
        }
    }
}
