using FluentAssertions;
using Moq;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Results.Common;
using RoomReservation.Core.Services;
using RoomReservation.Core.Tests.TestHelpers;

namespace RoomReservation.Core.Tests.Services
{
    public class ReservationExpiryServiceTests
    {
        private readonly Mock<IReservationRepository> _reservationsMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly ReservationExpiryService _sut;

        public ReservationExpiryServiceTests()
        {
            _unitOfWorkMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task<Result>>>()))
                           .Returns((Func<Task<Result>> action) => action());

            _sut = new(_reservationsMock.Object, _unitOfWorkMock.Object, new FixedTimeProvider());
        }

        private static Reservation CreatePendingRequest(TimeOnly start) => new()
        {
            RoomId = Guid.NewGuid(),
            Date = new DateOnly(2030, 1, 15),
            StartTime = start,
            EndTime = start.AddHours(1),
            Status = ReservationStatus.Pending
        };

        [Fact]
        public async Task ExpireStartedRequestsAsync_MarksLockedRequestsAsExpired()
        {
            var first = CreatePendingRequest(new TimeOnly(9, 0));
            var second = CreatePendingRequest(new TimeOnly(10, 0));
            _reservationsMock.Setup(x => x.LockStartedPendingAsync(It.IsAny<DateTime>()))
                             .ReturnsAsync([first, second]);

            var expiredCount = await _sut.ExpireStartedRequestsAsync();

            expiredCount.Should().Be(2);
            first.Status.Should().Be(ReservationStatus.Expired);
            second.Status.Should().Be(ReservationStatus.Expired);
            _unitOfWorkMock.Verify(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task<Result>>>()), Times.Once);
        }

        [Fact]
        public async Task ExpireStartedRequestsAsync_PassesCurrentWarsawTime()
        {
            _reservationsMock.Setup(x => x.LockStartedPendingAsync(It.IsAny<DateTime>()))
                             .ReturnsAsync([]);

            var expiredCount = await _sut.ExpireStartedRequestsAsync();

            expiredCount.Should().Be(0);
            _reservationsMock.Verify(x => x.LockStartedPendingAsync(new DateTime(2030, 1, 15, 11, 0, 0)), Times.Once);
        }
    }
}
