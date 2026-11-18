using FluentAssertions;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Services;

namespace RoomReservation.Core.Tests.Services
{
    public class ReservationVisibilityTests
    {
        private static readonly Guid UserId = Guid.NewGuid();
        private static readonly Guid OtherUserId = Guid.NewGuid();

        private static Reservation CreateReservation(Guid authorId, ReservationStatus status) => new()
        {
            RoomId = Guid.NewGuid(),
            CreatedById = authorId,
            Date = new DateOnly(2030, 1, 16),
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0),
            Status = status
        };

        [Fact]
        public void VisibleInRoomCalendar_WithoutModeration_HidesOnlyOtherUsersRequests()
        {
            var ownRequest = CreateReservation(UserId, ReservationStatus.Pending);
            var otherRequest = CreateReservation(OtherUserId, ReservationStatus.Pending);
            var otherApproved = CreateReservation(OtherUserId, ReservationStatus.Approved);

            var visible = ReservationVisibility.VisibleInRoomCalendar([ownRequest, otherRequest, otherApproved], UserId, canSeeAllRequests: false);

            visible.Should().BeEquivalentTo([ownRequest, otherApproved]);
        }

        [Fact]
        public void VisibleInRoomCalendar_WithModeration_ShowsEverything()
        {
            var otherRequest = CreateReservation(OtherUserId, ReservationStatus.Pending);
            var otherApproved = CreateReservation(OtherUserId, ReservationStatus.Approved);

            var visible = ReservationVisibility.VisibleInRoomCalendar([otherRequest, otherApproved], UserId, canSeeAllRequests: true);

            visible.Should().HaveCount(2);
        }
    }
}
