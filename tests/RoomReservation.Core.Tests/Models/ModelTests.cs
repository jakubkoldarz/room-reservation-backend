using FluentAssertions;
using RoomReservation.Core.Models;
using RoomReservation.Core.Models.Availability;

namespace RoomReservation.Core.Tests.Models
{
    public class ModelTests
    {
        [Theory]
        [InlineData(8, 10, true)]
        [InlineData(7, 10, false)]
        [InlineData(15, 17, false)]
        [InlineData(8, 16, true)]
        public void AvailabilityResolution_Covers_ChecksIfRangeFitsInOpeningHours(int from, int to, bool expected)
        {
            var resolution = new AvailabilityResolution(false, new TimeOnly(8, 0), new TimeOnly(16, 0));

            resolution.Covers(new TimeOnly(from, 0), new TimeOnly(to, 0)).Should().Be(expected);
        }

        [Fact]
        public void AvailabilityResolution_Covers_WhenClosed_ReturnsFalse()
        {
            var resolution = new AvailabilityResolution(true, null, null);

            resolution.Covers(new TimeOnly(9, 0), new TimeOnly(10, 0)).Should().BeFalse();
        }

        [Fact]
        public void UserAccessModel_HasPermission_SuperAdminHasEveryPermission()
        {
            var access = new UserAccessModel(true, true, new HashSet<string>());

            access.HasPermission("anything").Should().BeTrue();
        }

        [Fact]
        public void UserAccessModel_HasPermission_RegularUserNeedsExplicitPermission()
        {
            var access = new UserAccessModel(true, false, new HashSet<string> { "room.view" });

            access.HasPermission("room.view").Should().BeTrue();
            access.HasPermission("room.delete").Should().BeFalse();
        }

        [Theory]
        [InlineData(0, 1, 0, false)]
        [InlineData(25, 1, 3, true)]
        [InlineData(25, 3, 3, false)]
        public void PagedList_CalculatesPages(int totalCount, int page, int expectedTotalPages, bool expectedHasNextPage)
        {
            var list = new PagedList<int>([], totalCount, page, 10);

            list.TotalPages.Should().Be(expectedTotalPages);
            list.HasNextPage.Should().Be(expectedHasNextPage);
        }
    }
}
