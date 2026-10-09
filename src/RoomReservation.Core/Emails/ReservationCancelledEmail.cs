using RoomReservation.Core.Providers;

namespace RoomReservation.Core.Emails
{
    public class ReservationCancelledEmail : ReservationEmail
    {
        public required string CancelledBy { get; init; }
        public required DateTime CancelledAt { get; init; }
        public required string CancelReason { get; init; }

        internal override string Subject => "Twoja rezerwacja została anulowana";

        internal override Dictionary<string, string> GetReplacements()
        {
            var replacements = GetReservationReplacements();
            replacements[nameof(CancelledBy)] = CancelledBy;
            replacements[nameof(CancelledAt)] = TimeZoneProvider.ToWarsawTime(CancelledAt).ToString("HH:mm dd.MM.yyyy");
            replacements[nameof(CancelReason)] = CancelReason;
            return replacements;
        }
    }
}
