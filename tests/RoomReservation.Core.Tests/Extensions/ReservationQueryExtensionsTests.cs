using FluentAssertions;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Extensions;

namespace RoomReservation.Core.Tests.Extensions
{
    public class ReservationQueryExtensionsTests
    {
        private static readonly DateTime Now = new(2030, 1, 15, 14, 0, 0);
        private static readonly DateOnly Today = DateOnly.FromDateTime(Now);

        private static Reservation Reservation(DateOnly date, int startHour, ReservationStatus status = ReservationStatus.Approved) => new()
        {
            RoomId = Guid.NewGuid(),
            Date = date,
            StartTime = new TimeOnly(startHour, 0),
            EndTime = new TimeOnly(startHour + 1, 0),
            Status = status
        };

        private static List<Reservation> Filter(params Reservation[] reservations)
            => [.. reservations.AsQueryable().WhereActiveNotStarted(Now)];

        [Fact]
        public void WhereActiveNotStarted_ExcludesTodaysReservationsThatAlreadyStarted()
        {
            var finished = Reservation(Today, 8);
            var ongoing = Reservation(Today, 13);
            var startingNow = Reservation(Today, 14);
            var later = Reservation(Today, 15);

            Filter(finished, ongoing, startingNow, later).Should().ContainSingle().Which.Should().BeSameAs(later);
        }

        [Fact]
        public void WhereActiveNotStarted_IncludesFutureDaysAndExcludesPastDays()
        {
            var yesterday = Reservation(Today.AddDays(-1), 16);
            var tomorrowMorning = Reservation(Today.AddDays(1), 8);

            Filter(yesterday, tomorrowMorning).Should().ContainSingle().Which.Should().BeSameAs(tomorrowMorning);
        }

        [Theory]
        [InlineData(ReservationStatus.Pending, true)]
        [InlineData(ReservationStatus.Approved, true)]
        [InlineData(ReservationStatus.Rejected, false)]
        [InlineData(ReservationStatus.Canceled, false)]
        public void WhereActiveNotStarted_IncludesOnlyPendingAndApproved(ReservationStatus status, bool included)
        {
            var reservation = Reservation(Today.AddDays(1), 10, status);

            Filter(reservation).Should().HaveCount(included ? 1 : 0);
        }
    }
}
