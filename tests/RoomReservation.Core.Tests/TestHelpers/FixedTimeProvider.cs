namespace RoomReservation.Core.Tests.TestHelpers
{
    public class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public static readonly DateTimeOffset DefaultNow = new(2030, 1, 15, 10, 0, 0, TimeSpan.Zero);

        public DateTimeOffset UtcNow { get; set; } = utcNow;

        public FixedTimeProvider() : this(DefaultNow) { }

        public override DateTimeOffset GetUtcNow() => UtcNow;
    }
}
