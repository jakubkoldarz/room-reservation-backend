using RoomReservation.Core.Providers;

namespace RoomReservation.Core.Extensions
{
    internal static class TimeProviderExtensions
    {
        public static DateTime UtcNow(this TimeProvider timeProvider)
            => timeProvider.GetUtcNow().UtcDateTime;

        public static DateTime WarsawNow(this TimeProvider timeProvider)
            => TimeZoneProvider.ToWarsawTime(timeProvider.UtcNow());

        public static DateOnly WarsawToday(this TimeProvider timeProvider)
            => DateOnly.FromDateTime(timeProvider.WarsawNow());
    }
}
