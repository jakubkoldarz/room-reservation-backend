using FluentAssertions;
using Moq;
using RoomReservation.Core.Emails;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models.Availability;
using RoomReservation.Core.Results.Common;
using RoomReservation.Core.Services;
using RoomReservation.Core.Tests.TestHelpers;

namespace RoomReservation.Core.Tests.Services
{
    public class ReservationServiceTests
    {
        private readonly Mock<IReservationRepository> _reservationsMock = new();
        private readonly Mock<IUserRepository> _usersMock = new();
        private readonly Mock<IAvailabilityService> _availabilityServiceMock = new();
        private readonly Mock<IEmailQueue> _emailQueueMock = new();
        private readonly Mock<IRoomRepository> _roomsMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

        private readonly ReservationService _sut;

        private static readonly DateOnly Today = new(2030, 1, 15);
        private static readonly DateOnly Tomorrow = Today.AddDays(1);

        private readonly User _owner = UserFaker.Create(email: "owner@test.com");
        private readonly Room _room;

        public ReservationServiceTests()
        {
            var building = new Building { Name = "Budynek A", Identifier = "A", Street = "s", City = "c", PostalCode = "p", FloorsCount = 1 };
            _room = new Room { Identifier = "A-101", RequiresApproval = false, Floor = 1, Capacity = 20, Building = building, BuildingId = building.Id };

            _sut = new(_reservationsMock.Object,
                       _usersMock.Object,
                       _availabilityServiceMock.Object,
                       _emailQueueMock.Object,
                       _roomsMock.Object,
                       _unitOfWorkMock.Object,
                       new FixedTimeProvider());

            _roomsMock.Setup(x => x.GetByIdAsync(_room.Id))
                      .ReturnsAsync(_room);
            _availabilityServiceMock.Setup(x => x.ResolveAvailabilityAsync(_room.Id, It.IsAny<DateOnly>()))
                                    .ReturnsAsync(new AvailabilityResolution(false, new TimeOnly(8, 0), new TimeOnly(20, 0)));
            _unitOfWorkMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task<Result>>>()))
                           .Returns((Func<Task<Result>> action) => action());
            _reservationsMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                             .ReturnsAsync((Guid id) => CreateReservation(new TimeOnly(0, 0), new TimeOnly(0, 0), id));
            _reservationsMock.Setup(x => x.GetActiveByRoomAndDateAsync(_room.Id, It.IsAny<DateOnly>()))
                             .ReturnsAsync([]);
        }

        private Reservation CreateReservation(
            TimeOnly start,
            TimeOnly end,
            Guid? id = null,
            ReservationStatus status = ReservationStatus.Approved,
            DateOnly? date = null) => new()
        {
            Id = id ?? Guid.NewGuid(),
            RoomId = _room.Id,
            Room = _room,
            CreatedById = _owner.Id,
            CreatedBy = _owner,
            Date = date ?? Tomorrow,
            StartTime = start,
            EndTime = end,
            Status = status
        };

        private void SetupExistingReservations(params Reservation[] reservations)
            => _reservationsMock.Setup(x => x.GetActiveByRoomAndDateAsync(_room.Id, Tomorrow))
                                .ReturnsAsync(reservations);

        private Reservation SetupReservation(ReservationStatus status)
        {
            var reservation = CreateReservation(new TimeOnly(10, 0), new TimeOnly(11, 0), status: status);
            _reservationsMock.Setup(x => x.GetByIdAsync(reservation.Id)).ReturnsAsync(reservation);
            return reservation;
        }

        #region CreateAsync

        [Fact]
        public async Task CreateAsync_WhenStartIsNotBeforeEnd_ReturnsBadRequest()
        {
            var result = await _sut.CreateAsync(_room.Id, Tomorrow, new TimeOnly(12, 0), new TimeOnly(12, 0), _owner.Id);

            result.Error!.ErrorType.Should().Be(ErrorType.BadRequest);
        }

        [Fact]
        public async Task CreateAsync_WhenDateIsInThePast_ReturnsBadRequest()
        {
            var result = await _sut.CreateAsync(_room.Id, Today.AddDays(-1), new TimeOnly(12, 0), new TimeOnly(13, 0), _owner.Id);

            result.Error!.ErrorType.Should().Be(ErrorType.BadRequest);
        }

        [Fact]
        public async Task CreateAsync_WhenTodayButHourAlreadyPassedInWarsaw_ReturnsBadRequest()
        {
            // FixedTimeProvider: 10:00 UTC = 11:00 w Warszawie
            var result = await _sut.CreateAsync(_room.Id, Today, new TimeOnly(10, 30), new TimeOnly(12, 0), _owner.Id);

            result.Error!.ErrorType.Should().Be(ErrorType.BadRequest);
        }

        [Fact]
        public async Task CreateAsync_WhenRoomDoesNotExist_ReturnsNotFound()
        {
            var result = await _sut.CreateAsync(Guid.NewGuid(), Tomorrow, new TimeOnly(12, 0), new TimeOnly(13, 0), _owner.Id);

            result.Error!.ErrorType.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task CreateAsync_WhenOutsideRoomAvailability_ReturnsBadRequest()
        {
            var result = await _sut.CreateAsync(_room.Id, Tomorrow, new TimeOnly(19, 0), new TimeOnly(21, 0), _owner.Id);

            result.Error!.ErrorType.Should().Be(ErrorType.BadRequest);
            _reservationsMock.Verify(x => x.Add(It.IsAny<Reservation>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenOnlyOneOfExistingReservationsOverlaps_ReturnsConflict()
        {
            SetupExistingReservations(
                CreateReservation(new TimeOnly(8, 0), new TimeOnly(9, 0)),
                CreateReservation(new TimeOnly(10, 0), new TimeOnly(12, 0)));

            var result = await _sut.CreateAsync(_room.Id, Tomorrow, new TimeOnly(11, 0), new TimeOnly(13, 0), _owner.Id);

            result.Error!.ErrorType.Should().Be(ErrorType.Conflict);
            _reservationsMock.Verify(x => x.Add(It.IsAny<Reservation>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenReservationTouchesExistingOne_Succeeds()
        {
            SetupExistingReservations(CreateReservation(new TimeOnly(10, 0), new TimeOnly(12, 0)));

            var result = await _sut.CreateAsync(_room.Id, Tomorrow, new TimeOnly(12, 0), new TimeOnly(13, 0), _owner.Id);

            result.IsSuccess.Should().BeTrue();
            _reservationsMock.Verify(x => x.LockRoomAsync(_room.Id), Times.Once);
            _reservationsMock.Verify(x => x.Add(It.IsAny<Reservation>()), Times.Once);
        }

        [Theory]
        [InlineData(true, ReservationStatus.Pending)]
        [InlineData(false, ReservationStatus.Approved)]
        public async Task CreateAsync_SetsStatusBasedOnRoomApprovalRequirement(bool requiresApproval, ReservationStatus expectedStatus)
        {
            _room.RequiresApproval = requiresApproval;
            Reservation? added = null;
            _reservationsMock.Setup(x => x.Add(It.IsAny<Reservation>())).Callback<Reservation>(r => added = r);

            await _sut.CreateAsync(_room.Id, Tomorrow, new TimeOnly(12, 0), new TimeOnly(13, 0), _owner.Id);

            added!.Status.Should().Be(expectedStatus);
        }

        #endregion
        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_WhenItIsTheOnlyReservationThatDay_Succeeds()
        {
            var reservation = CreateReservation(new TimeOnly(10, 0), new TimeOnly(11, 0));
            _reservationsMock.Setup(x => x.GetByIdAsync(reservation.Id)).ReturnsAsync(reservation);
            SetupExistingReservations(reservation);

            var result = await _sut.UpdateAsync(_owner.Id, reservation.Id, new TimeOnly(10, 30), new TimeOnly(12, 0));

            result.IsSuccess.Should().BeTrue();
            reservation.StartTime.Should().Be(new TimeOnly(10, 30));
            reservation.EndTime.Should().Be(new TimeOnly(12, 0));
        }

        [Fact]
        public async Task UpdateAsync_WhenUserIsNotOwner_ReturnsNotFound()
        {
            var reservation = SetupReservation(ReservationStatus.Approved);

            var result = await _sut.UpdateAsync(Guid.NewGuid(), reservation.Id, new TimeOnly(12, 0), new TimeOnly(13, 0));

            result.Error!.ErrorType.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task UpdateAsync_WhenTimeChangesInRoomRequiringApproval_ResetsStatusToPending()
        {
            _room.RequiresApproval = true;
            var reservation = SetupReservation(ReservationStatus.Approved);

            var result = await _sut.UpdateAsync(_owner.Id, reservation.Id, new TimeOnly(12, 0), new TimeOnly(13, 0));

            result.IsSuccess.Should().BeTrue();
            reservation.Status.Should().Be(ReservationStatus.Pending);
        }

        #endregion
        #region ApproveAsync / RejectAsync

        [Fact]
        public async Task ApproveAsync_WhenReservationIsNotPending_ReturnsBadRequest()
        {
            var reservation = SetupReservation(ReservationStatus.Approved);

            var result = await _sut.ApproveAsync(reservation.Id, Guid.NewGuid());

            result.Error!.ErrorType.Should().Be(ErrorType.BadRequest);
        }

        [Fact]
        public async Task ApproveAsync_WhenPending_ApprovesNotifiesOwnerAndSaves()
        {
            var reservation = SetupReservation(ReservationStatus.Pending);
            var approverId = Guid.NewGuid();

            var result = await _sut.ApproveAsync(reservation.Id, approverId);

            result.IsSuccess.Should().BeTrue();
            reservation.Status.Should().Be(ReservationStatus.Approved);
            reservation.ApprovedById.Should().Be(approverId);
            reservation.ApprovedAt.Should().Be(FixedTimeProvider.DefaultNow.UtcDateTime);
            _emailQueueMock.Verify(x => x.Enqueue(It.Is<ReservationApprovedEmail>(e => e.To == _owner.Email)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RejectAsync_WhenPending_RejectsWithReasonAndNotifiesOwner()
        {
            var reservation = SetupReservation(ReservationStatus.Pending);

            var result = await _sut.RejectAsync(reservation.Id, "Sala zajęta", Guid.NewGuid());

            result.IsSuccess.Should().BeTrue();
            reservation.Status.Should().Be(ReservationStatus.Rejected);
            reservation.Reason.Should().Be("Sala zajęta");
            _emailQueueMock.Verify(x => x.Enqueue(It.Is<ReservationRejectedEmail>(e => e.RejectReason == "Sala zajęta")), Times.Once);
        }

        #endregion
        #region Cancel / Delete

        [Fact]
        public async Task SelfCancelAsync_WhenUserIsNotOwner_ReturnsNotFound()
        {
            var reservation = SetupReservation(ReservationStatus.Approved);

            var result = await _sut.SelfCancelAsync(reservation.Id, null, Guid.NewGuid());

            result.Error!.ErrorType.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task SelfCancelAsync_WhenApproved_CancelsWithoutNotification()
        {
            var reservation = SetupReservation(ReservationStatus.Approved);

            var result = await _sut.SelfCancelAsync(reservation.Id, "Zmiana planów", _owner.Id);

            result.IsSuccess.Should().BeTrue();
            reservation.Status.Should().Be(ReservationStatus.Canceled);
            reservation.CanceledById.Should().Be(_owner.Id);
            _emailQueueMock.Verify(x => x.Enqueue(It.IsAny<EmailMessage>()), Times.Never);
        }

        [Fact]
        public async Task ForceCancelAsync_WhenActive_CancelsAndNotifiesOwner()
        {
            var reservation = SetupReservation(ReservationStatus.Pending);

            var result = await _sut.ForceCancelAsync(reservation.Id, "Awaria", Guid.NewGuid());

            result.IsSuccess.Should().BeTrue();
            reservation.Status.Should().Be(ReservationStatus.Canceled);
            _emailQueueMock.Verify(x => x.Enqueue(It.IsAny<ReservationCancelledEmail>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ForceCancelAsync_WhenAlreadyCanceled_ReturnsBadRequest()
        {
            var reservation = SetupReservation(ReservationStatus.Canceled);

            var result = await _sut.ForceCancelAsync(reservation.Id, null);

            result.Error!.ErrorType.Should().Be(ErrorType.BadRequest);
        }

        [Fact]
        public async Task DeleteAsync_WhenNotPending_ReturnsBadRequest()
        {
            var reservation = SetupReservation(ReservationStatus.Approved);

            var result = await _sut.DeleteAsync(reservation.Id, _owner.Id);

            result.Error!.ErrorType.Should().Be(ErrorType.BadRequest);
            _reservationsMock.Verify(x => x.Remove(It.IsAny<Reservation>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenPending_RemovesAndSaves()
        {
            var reservation = SetupReservation(ReservationStatus.Pending);

            var result = await _sut.DeleteAsync(reservation.Id, _owner.Id);

            result.IsSuccess.Should().BeTrue();
            _reservationsMock.Verify(x => x.Remove(reservation), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion
        #region BulkForceCancelAsync

        [Fact]
        public async Task BulkForceCancelAsync_WhenListIsEmpty_DoesNothing()
        {
            var result = await _sut.BulkForceCancelAsync([], "powód");

            result.IsSuccess.Should().BeTrue();
            _reservationsMock.Verify(x => x.GetByIdsAsync(It.IsAny<IReadOnlyList<Guid>>()), Times.Never);
        }

        [Fact]
        public async Task BulkForceCancelAsync_CancelsOnlyActiveReservationsAndDoesNotSave()
        {
            var active = CreateReservation(new TimeOnly(8, 0), new TimeOnly(9, 0), status: ReservationStatus.Approved);
            var rejected = CreateReservation(new TimeOnly(10, 0), new TimeOnly(11, 0), status: ReservationStatus.Rejected);
            _reservationsMock.Setup(x => x.GetByIdsAsync(It.IsAny<IReadOnlyList<Guid>>())).ReturnsAsync([active, rejected]);

            await _sut.BulkForceCancelAsync([active, rejected], "Zmiany administracyjne");

            active.Status.Should().Be(ReservationStatus.Canceled);
            rejected.Status.Should().Be(ReservationStatus.Rejected);
            _emailQueueMock.Verify(x => x.Enqueue(It.IsAny<ReservationCancelledEmail>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        #endregion
        #region GetConflictingWithEventAsync

        [Fact]
        public async Task GetConflictingWithEventAsync_WhenEventIsOpen_ReturnsReservationsOutsideEventHours()
        {
            var inside = CreateReservation(new TimeOnly(10, 0), new TimeOnly(11, 0));
            var outside = CreateReservation(new TimeOnly(15, 0), new TimeOnly(16, 0));
            var otherDay = CreateReservation(new TimeOnly(15, 0), new TimeOnly(16, 0), date: Tomorrow.AddDays(5));
            _reservationsMock.Setup(x => x.GetActiveFutureByRoomIdsAsync(It.IsAny<IReadOnlyList<Guid>>())).ReturnsAsync([inside, outside, otherDay]);

            var ev = new Event
            {
                Name = "Konferencja",
                StartDate = Tomorrow,
                EndDate = Tomorrow,
                IsClosed = false,
                StartTime = new TimeOnly(9, 0),
                EndTime = new TimeOnly(12, 0),
                Rooms = [_room]
            };

            var result = await _sut.GetConflictingWithEventAsync(ev);

            result.Should().ContainSingle().Which.Should().BeSameAs(outside);
        }

        [Fact]
        public async Task GetConflictingWithEventAsync_WhenEventIsClosed_ReturnsAllReservationsInRange()
        {
            var first = CreateReservation(new TimeOnly(10, 0), new TimeOnly(11, 0));
            var second = CreateReservation(new TimeOnly(15, 0), new TimeOnly(16, 0));
            _reservationsMock.Setup(x => x.GetActiveFutureByRoomIdsAsync(It.IsAny<IReadOnlyList<Guid>>())).ReturnsAsync([first, second]);

            var ev = new Event { Name = "Remont", StartDate = Tomorrow, EndDate = Tomorrow, IsClosed = true, Rooms = [_room] };

            var result = await _sut.GetConflictingWithEventAsync(ev);

            result.Should().HaveCount(2);
        }

        #endregion
    }
}
