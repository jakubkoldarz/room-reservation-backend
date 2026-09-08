namespace RoomReservation.Core.Providers
{
    internal static class TimeZoneProvider
    {
        private static readonly TimeZoneInfo WarsawZone =
            TimeZoneInfo.FindSystemTimeZoneById("Europe/Warsaw");

        public static DateTime ToWarsawTime(DateTime utc) =>
            TimeZoneInfo.ConvertTimeFromUtc(utc, WarsawZone);
    }
}
