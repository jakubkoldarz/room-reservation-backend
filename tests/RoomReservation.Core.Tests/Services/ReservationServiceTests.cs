using FluentAssertions;
using Moq;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models.Availability;
using RoomReservation.Core.Results.Common;
using RoomReservation.Core.Services;

namespace RoomReservation.Core.Tests.Services
{
    public class ReservationServiceTests
    {
        private readonly Mock<IReservationRepository> _reservationsMock = new();
        private readonly Mock<IUserRepository> _usersMock = new();
        private readonly Mock<IAvailabilityService> _availabilityServiceMock = new();
        private readonly Mock<IEmailService> _emailServiceMock = new();
        private readonly Mock<IRoomRepository> _roomsMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

        private readonly ReservationService _sut;

        private static readonly DateOnly Tomorrow = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);
        private readonly Room _room = new() { Identifier = "A-101", RequiresApproval = false, Floor = 1, Capacity = 20 };

        public ReservationServiceTests()
        {
            _sut = new(_reservationsMock.Object,
                       _usersMock.Object,
                       _availabilityServiceMock.Object,
                       _emailServiceMock.Object,
                       _roomsMock.Object,
                       _unitOfWorkMock.Object);

            _roomsMock.Setup(x => x.GetByIdAsync(_room.Id))
                      .ReturnsAsync(_room);
            _availabilityServiceMock.Setup(x => x.ResolveAvailabilityAsync(_room.Id, It.IsAny<DateOnly>()))
                                    .ReturnsAsync(new AvailabilityResolution(false, new TimeOnly(8, 0), new TimeOnly(20, 0)));
            _unitOfWorkMock.Setup(x => x.ExecuteInTransactionAsync(It.IsAny<Func<Task<Result>>>()))
                           .Returns((Func<Task<Result>> action) => action());
            _reservationsMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                             .ReturnsAsync((Guid id) => CreateReservation(new TimeOnly(0, 0), new TimeOnly(0, 0), id));
        }

        private Reservation CreateReservation(TimeOnly start, TimeOnly end, Guid? id = null, Guid? createdById = null) => new()
        {
            Id = id ?? Guid.NewGuid(),
            RoomId = _room.Id,
            Room = _room,
            CreatedById = createdById,
            Date = Tomorrow,
            StartTime = start,
            EndTime = end,
            Status = ReservationStatus.Approved
        };

        private void SetupExistingReservations(params Reservation[] reservations)
            => _reservationsMock.Setup(x => x.GetActiveByRoomAndDateAsync(_room.Id, Tomorrow))
                                .ReturnsAsync(reservations);

        #region CreateAsync

        [Fact]
        public async Task CreateAsync_WhenOnlyOneOfExistingReservationsOverlaps_ReturnsConflict()
        {
            SetupExistingReservations(
                CreateReservation(new TimeOnly(8, 0), new TimeOnly(9, 0)),
                CreateReservation(new TimeOnly(10, 0), new TimeOnly(12, 0)));

            var result = await _sut.CreateAsync(_room.Id, Tomorrow, new TimeOnly(11, 0), new TimeOnly(13, 0), Guid.NewGuid());

            result.IsSuccess.Should().BeFalse();
            result.Error!.ErrorType.Should().Be(ErrorType.Conflict);
            _reservationsMock.Verify(x => x.Add(It.IsAny<Reservation>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenReservationTouchesExistingOne_Succeeds()
        {
            SetupExistingReservations(CreateReservation(new TimeOnly(10, 0), new TimeOnly(12, 0)));

            var result = await _sut.CreateAsync(_room.Id, Tomorrow, new TimeOnly(12, 0), new TimeOnly(13, 0), Guid.NewGuid());

            result.IsSuccess.Should().BeTrue();
            _reservationsMock.Verify(x => x.LockRoomAsync(_room.Id), Times.Once);
            _reservationsMock.Verify(x => x.Add(It.IsAny<Reservation>()), Times.Once);
        }

        #endregion
        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_WhenItIsTheOnlyReservationThatDay_Succeeds()
        {
            var userId = Guid.NewGuid();
            var reservation = CreateReservation(new TimeOnly(10, 0), new TimeOnly(11, 0), createdById: userId);

            _reservationsMock.Setup(x => x.GetByIdAsync(reservation.Id))
                             .ReturnsAsync(reservation);
            SetupExistingReservations(reservation);

            var result = await _sut.UpdateAsync(userId, reservation.Id, new TimeOnly(10, 30), new TimeOnly(12, 0));

            result.IsSuccess.Should().BeTrue();
            reservation.StartTime.Should().Be(new TimeOnly(10, 30));
            reservation.EndTime.Should().Be(new TimeOnly(12, 0));
        }

        #endregion
    }
}
