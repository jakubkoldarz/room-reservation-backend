using FluentAssertions;
using Moq;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models.Availability;
using RoomReservation.Core.Models.Reservations;
using RoomReservation.Core.Models.Rooms;
using RoomReservation.Core.Results.Common;
using RoomReservation.Core.Services;

namespace RoomReservation.Core.Tests.Services
{
    public class AvailabilityServiceTests
    {
        private readonly Mock<IAvailabilityRepository> _availabilitiesMock = new();
        private readonly Mock<IReservationRepository> _reservationsMock = new();
        private readonly Mock<IEventRepository> _eventsMock = new();
        private readonly Mock<IRoomRepository> _roomsMock = new();

        private readonly AvailabilityService _sut;

        private static readonly DateOnly Monday = new(2030, 1, 14);
        private readonly Room _room = new() { Identifier = "101", RequiresApproval = false, Floor = 1, Capacity = 10 };

        public AvailabilityServiceTests()
        {
            _sut = new(_availabilitiesMock.Object, _reservationsMock.Object, _eventsMock.Object, _roomsMock.Object);

            _eventsMock.Setup(x => x.GetActiveByRoomAsync(It.IsAny<Guid>())).ReturnsAsync([]);
            _availabilitiesMock.Setup(x => x.GetByRoomAsync(It.IsAny<Guid>())).ReturnsAsync([]);
            _reservationsMock.Setup(x => x.GetActiveFutureByRoomAsync(It.IsAny<Guid>())).ReturnsAsync([]);
            _roomsMock.Setup(x => x.GetByBuildingIdAsync(It.IsAny<Guid>())).ReturnsAsync([]);
        }

        private static Availability Slot(DayOfWeek day, int from, int to, Guid? roomId = null)
            => new() { DayOfWeek = day, StartTime = new TimeOnly(from, 0), EndTime = new TimeOnly(to, 0), RoomId = roomId };

        private Reservation ReservationOn(DateOnly date, int from, int to)
            => new() { RoomId = _room.Id, Date = date, StartTime = new TimeOnly(from, 0), EndTime = new TimeOnly(to, 0), Status = ReservationStatus.Approved };

        private Event EventOn(DateOnly date, bool isClosed, int? from = null, int? to = null) => new()
        {
            Name = "Egzamin",
            StartDate = date,
            EndDate = date,
            IsClosed = isClosed,
            StartTime = from is null ? null : new TimeOnly(from.Value, 0),
            EndTime = to is null ? null : new TimeOnly(to.Value, 0),
            Rooms = [_room]
        };

        #region AreAvailabilitiesValid

        [Fact]
        public async Task AreAvailabilitiesValid_WhenListIsEmpty_ReturnsBadRequest()
        {
            var result = await _sut.AreAvailabilitiesValid([]);

            result.Error!.ErrorType.Should().Be(ErrorType.BadRequest);
        }

        [Fact]
        public async Task AreAvailabilitiesValid_WhenStartIsNotBeforeEnd_ReturnsBadRequest()
        {
            var result = await _sut.AreAvailabilitiesValid([Slot(DayOfWeek.Monday, 12, 12)]);

            result.Error!.ErrorType.Should().Be(ErrorType.BadRequest);
        }

        [Fact]
        public async Task AreAvailabilitiesValid_WhenDayIsDuplicated_ReturnsConflict()
        {
            var result = await _sut.AreAvailabilitiesValid([Slot(DayOfWeek.Monday, 8, 10), Slot(DayOfWeek.Monday, 12, 14)]);

            result.Error!.ErrorType.Should().Be(ErrorType.Conflict);
        }

        [Fact]
        public async Task AreAvailabilitiesValid_WhenOutsideBuildingHours_ReturnsConflict()
        {
            var buildingId = Guid.NewGuid();
            _availabilitiesMock.Setup(x => x.GetByBuildingAsync(buildingId)).ReturnsAsync([Slot(DayOfWeek.Monday, 8, 16)]);

            var result = await _sut.AreAvailabilitiesValid([Slot(DayOfWeek.Monday, 7, 12)], boundingBuildingId: buildingId);

            result.Error!.ErrorType.Should().Be(ErrorType.Conflict);
        }

        [Fact]
        public async Task AreAvailabilitiesValid_WhenWithinBuildingHours_Succeeds()
        {
            var buildingId = Guid.NewGuid();
            _availabilitiesMock.Setup(x => x.GetByBuildingAsync(buildingId)).ReturnsAsync([Slot(DayOfWeek.Monday, 8, 16)]);

            var result = await _sut.AreAvailabilitiesValid([Slot(DayOfWeek.Monday, 9, 15)], boundingBuildingId: buildingId);

            result.IsSuccess.Should().BeTrue();
        }

        #endregion
        #region IsWithinBuildingBounds

        [Fact]
        public void IsWithinBuildingBounds_WhenBuildingIsClosedThatDay_ReturnsFalse()
        {
            var result = _sut.IsWithinBuildingBounds([Slot(DayOfWeek.Sunday, 9, 10)], [Slot(DayOfWeek.Monday, 8, 16)]);

            result.Should().BeFalse();
        }

        #endregion
        #region ResolveAvailabilityAsync

        [Fact]
        public async Task ResolveAvailabilityAsync_WhenNoEvent_UsesWeeklyAvailability()
        {
            _availabilitiesMock.Setup(x => x.GetByRoomAsync(_room.Id)).ReturnsAsync([Slot(DayOfWeek.Monday, 8, 16, _room.Id)]);

            var resolution = await _sut.ResolveAvailabilityAsync(_room.Id, Monday);

            resolution.IsClosed.Should().BeFalse();
            resolution.StartTime.Should().Be(new TimeOnly(8, 0));
            resolution.EndTime.Should().Be(new TimeOnly(16, 0));
        }

        [Fact]
        public async Task ResolveAvailabilityAsync_WhenNoAvailabilityForThatDay_ReturnsClosed()
        {
            _availabilitiesMock.Setup(x => x.GetByRoomAsync(_room.Id)).ReturnsAsync([Slot(DayOfWeek.Tuesday, 8, 16, _room.Id)]);

            var resolution = await _sut.ResolveAvailabilityAsync(_room.Id, Monday);

            resolution.IsClosed.Should().BeTrue();
        }

        [Fact]
        public async Task ResolveAvailabilityAsync_WhenClosedEventThatDay_ReturnsClosed()
        {
            _availabilitiesMock.Setup(x => x.GetByRoomAsync(_room.Id)).ReturnsAsync([Slot(DayOfWeek.Monday, 8, 16, _room.Id)]);
            _eventsMock.Setup(x => x.GetActiveByRoomAsync(_room.Id)).ReturnsAsync([EventOn(Monday, isClosed: true)]);

            var resolution = await _sut.ResolveAvailabilityAsync(_room.Id, Monday);

            resolution.IsClosed.Should().BeTrue();
        }

        [Fact]
        public async Task ResolveAvailabilityAsync_WhenOpenEventThatDay_UsesEventHours()
        {
            _availabilitiesMock.Setup(x => x.GetByRoomAsync(_room.Id)).ReturnsAsync([Slot(DayOfWeek.Monday, 8, 16, _room.Id)]);
            _eventsMock.Setup(x => x.GetActiveByRoomAsync(_room.Id)).ReturnsAsync([EventOn(Monday, isClosed: false, from: 10, to: 12)]);

            var resolution = await _sut.ResolveAvailabilityAsync(_room.Id, Monday);

            resolution.StartTime.Should().Be(new TimeOnly(10, 0));
            resolution.EndTime.Should().Be(new TimeOnly(12, 0));
        }

        #endregion
        #region GetConflictingReservationsForRoomAsync

        [Fact]
        public async Task GetConflictingReservationsForRoomAsync_ReturnsOnlyReservationsOutsideNewHours()
        {
            var inside = ReservationOn(Monday, 9, 10);
            var outside = ReservationOn(Monday, 15, 17);
            _reservationsMock.Setup(x => x.GetActiveFutureByRoomAsync(_room.Id)).ReturnsAsync([inside, outside]);

            var result = await _sut.GetConflictingReservationsForRoomAsync(_room.Id, [Slot(DayOfWeek.Monday, 8, 16, _room.Id)], []);

            result.Should().ContainSingle().Which.Should().BeSameAs(outside);
        }

        #endregion
        #region ReplaceIfValidForRoomAsync

        [Fact]
        public async Task ReplaceIfValidForRoomAsync_WhenReservationsConflictAndNoForce_ReturnsConflictError()
        {
            _availabilitiesMock.Setup(x => x.GetByBuildingAsync(It.IsAny<Guid>())).ReturnsAsync([Slot(DayOfWeek.Monday, 6, 22)]);
            _reservationsMock.Setup(x => x.GetActiveFutureByRoomAsync(_room.Id)).ReturnsAsync([ReservationOn(Monday, 15, 17)]);

            var result = await _sut.ReplaceIfValidForRoomAsync(_room, [new AvailabilityModel(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(12, 0))]);

            result.IsSuccess.Should().BeFalse();
            result.Error.Should().BeOfType<ConflictError<ConflictingReservationModel>>()
                .Which.Items.Should().HaveCount(1);
            _availabilitiesMock.Verify(x => x.ReplaceForRoomAsync(It.IsAny<Guid>(), It.IsAny<IReadOnlyList<Availability>>()), Times.Never);
        }

        [Fact]
        public async Task ReplaceIfValidForRoomAsync_WhenForced_ReplacesAndReturnsConflicts()
        {
            var conflicting = ReservationOn(Monday, 15, 17);
            _availabilitiesMock.Setup(x => x.GetByBuildingAsync(It.IsAny<Guid>())).ReturnsAsync([Slot(DayOfWeek.Monday, 6, 22)]);
            _reservationsMock.Setup(x => x.GetActiveFutureByRoomAsync(_room.Id)).ReturnsAsync([conflicting]);

            var result = await _sut.ReplaceIfValidForRoomAsync(_room, [new AvailabilityModel(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(12, 0))], force: true);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().ContainSingle().Which.Should().BeSameAs(conflicting);
            _availabilitiesMock.Verify(x => x.ReplaceForRoomAsync(_room.Id, It.Is<IReadOnlyList<Availability>>(a => a.Count == 1 && a[0].RoomId == _room.Id)), Times.Once);
        }

        #endregion
        #region ReplaceIfValidForBuildingAsync

        [Fact]
        public async Task ReplaceIfValidForBuildingAsync_WhenRoomWouldExceedNewHours_ReturnsConflictError()
        {
            var building = new Building { Name = "A", Street = "s", City = "c", PostalCode = "p", FloorsCount = 1 };
            _room.Availabilities = [Slot(DayOfWeek.Monday, 7, 18, _room.Id)];
            _roomsMock.Setup(x => x.GetByBuildingIdAsync(building.Id)).ReturnsAsync([_room]);

            var result = await _sut.ReplaceIfValidForBuildingAsync(building, [new AvailabilityModel(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(16, 0))]);

            result.Error.Should().BeOfType<ConflictError<ConflictingRoomModel>>()
                .Which.Items.Should().ContainSingle(r => r.RoomId == _room.Id);
            _availabilitiesMock.Verify(x => x.ReplaceForBuildingAsync(It.IsAny<Guid>(), It.IsAny<IReadOnlyList<Availability>>()), Times.Never);
        }

        [Fact]
        public async Task ReplaceIfValidForBuildingAsync_WhenRoomsFit_ReplacesAvailabilities()
        {
            var building = new Building { Name = "A", Street = "s", City = "c", PostalCode = "p", FloorsCount = 1 };

            var result = await _sut.ReplaceIfValidForBuildingAsync(building, [new AvailabilityModel(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(16, 0))]);

            result.IsSuccess.Should().BeTrue();
            _availabilitiesMock.Verify(x => x.ReplaceForBuildingAsync(building.Id, It.Is<IReadOnlyList<Availability>>(a => a[0].BuildingId == building.Id)), Times.Once);
        }

        #endregion
    }
}
