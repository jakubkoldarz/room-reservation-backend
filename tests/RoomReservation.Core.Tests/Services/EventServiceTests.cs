using FluentAssertions;
using Moq;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models.Events;
using RoomReservation.Core.Models.Reservations;
using RoomReservation.Core.Results.Common;
using RoomReservation.Core.Services;
using RoomReservation.Core.Tests.TestHelpers;

namespace RoomReservation.Core.Tests.Services
{
    public class EventServiceTests
    {
        private readonly Mock<IEventRepository> _eventsMock = new();
        private readonly Mock<IAvailabilityRepository> _availabilitiesMock = new();
        private readonly Mock<IAvailabilityService> _availabilityServiceMock = new();
        private readonly Mock<IReservationService> _reservationServiceMock = new();
        private readonly Mock<IRoomRepository> _roomsMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

        private readonly EventService _sut;

        private static readonly DateOnly Today = new(2030, 1, 15);
        private readonly Room _room = new() { Identifier = "101", RequiresApproval = false, Floor = 1, Capacity = 10 };

        public EventServiceTests()
        {
            _sut = new(_eventsMock.Object,
                       _availabilitiesMock.Object,
                       _availabilityServiceMock.Object,
                       _reservationServiceMock.Object,
                       _roomsMock.Object,
                       _unitOfWorkMock.Object,
                       new FixedTimeProvider());

            _roomsMock.Setup(x => x.GetByIdsAsync(It.IsAny<IReadOnlyList<Guid>>())).ReturnsAsync([_room]);
            _eventsMock.Setup(x => x.GetActiveByRoomIdsAsync(It.IsAny<IReadOnlyList<Guid>>())).ReturnsAsync([]);
            _reservationServiceMock.Setup(x => x.GetConflictingWithEventAsync(It.IsAny<Event>())).ReturnsAsync([]);
            _reservationServiceMock.Setup(x => x.BulkForceCancelAsync(It.IsAny<IReadOnlyList<Reservation>>(), It.IsAny<string?>(), It.IsAny<Guid?>()))
                                   .ReturnsAsync(Result.Success());
        }

        private static EventModel ClosedEvent(DateOnly start, DateOnly end) => new("Remont", start, end, true, null, null);

        private Event ExistingEvent(DateOnly start, DateOnly end) => new()
        {
            Name = "Istniejące",
            StartDate = start,
            EndDate = end,
            IsClosed = true,
            Rooms = [_room]
        };

        private Reservation ReservationOn(DateOnly date) => new()
        {
            RoomId = _room.Id,
            Date = date,
            StartTime = new TimeOnly(10, 0),
            EndTime = new TimeOnly(11, 0),
            Status = ReservationStatus.Approved
        };

        #region CreateAsync

        [Fact]
        public async Task CreateAsync_WhenRoomIsMissing_ReturnsNotFound()
        {
            var result = await _sut.CreateAsync([_room.Id, Guid.NewGuid()], ClosedEvent(Today.AddDays(1), Today.AddDays(2)));

            result.Error!.ErrorType.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task CreateAsync_WhenClosedEventHasHours_ReturnsBadRequest()
        {
            var request = new EventModel("Remont", Today.AddDays(1), Today.AddDays(1), true, new TimeOnly(8, 0), new TimeOnly(10, 0));

            var result = await _sut.CreateAsync([_room.Id], request);

            result.Error!.ErrorType.Should().Be(ErrorType.BadRequest);
        }

        [Fact]
        public async Task CreateAsync_WhenOpenEventHasNoHours_ReturnsBadRequest()
        {
            var request = new EventModel("Dzień otwarty", Today.AddDays(1), Today.AddDays(1), false, null, null);

            var result = await _sut.CreateAsync([_room.Id], request);

            result.Error!.ErrorType.Should().Be(ErrorType.BadRequest);
        }

        [Fact]
        public async Task CreateAsync_WhenStartDateIsInThePast_ReturnsBadRequest()
        {
            var result = await _sut.CreateAsync([_room.Id], ClosedEvent(Today.AddDays(-1), Today.AddDays(1)));

            result.Error!.ErrorType.Should().Be(ErrorType.BadRequest);
        }

        [Fact]
        public async Task CreateAsync_WhenOverlapsExistingEvent_ReturnsConflictError()
        {
            _eventsMock.Setup(x => x.GetActiveByRoomIdsAsync(It.IsAny<IReadOnlyList<Guid>>()))
                       .ReturnsAsync([ExistingEvent(Today.AddDays(2), Today.AddDays(4))]);

            var result = await _sut.CreateAsync([_room.Id], ClosedEvent(Today.AddDays(3), Today.AddDays(5)));

            result.Error.Should().BeOfType<ConflictError<ConflictingEventModel>>();
            _eventsMock.Verify(x => x.Add(It.IsAny<Event>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenReservationsConflictAndNoForce_ReturnsConflictError()
        {
            _reservationServiceMock.Setup(x => x.GetConflictingWithEventAsync(It.IsAny<Event>()))
                                   .ReturnsAsync([ReservationOn(Today.AddDays(1))]);

            var result = await _sut.CreateAsync([_room.Id], ClosedEvent(Today.AddDays(1), Today.AddDays(1)));

            result.Error.Should().BeOfType<ConflictError<ConflictingReservationModel>>();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenForced_AddsEventAndCancelsConflictingReservations()
        {
            IReadOnlyList<Reservation> conflicts = [ReservationOn(Today.AddDays(1))];
            _reservationServiceMock.Setup(x => x.GetConflictingWithEventAsync(It.IsAny<Event>())).ReturnsAsync(conflicts);

            var result = await _sut.CreateAsync([_room.Id], ClosedEvent(Today.AddDays(1), Today.AddDays(1)), force: true);

            result.IsSuccess.Should().BeTrue();
            _eventsMock.Verify(x => x.Add(result.Value!), Times.Once);
            _reservationServiceMock.Verify(x => x.BulkForceCancelAsync(conflicts, It.IsAny<string?>(), It.IsAny<Guid?>()), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion
        #region GetConflictingEvents

        [Fact]
        public void GetConflictingEvents_IgnoresEventsInOtherRoomsAndExcludedEvent()
        {
            var otherRoom = new Room { Identifier = "202", RequiresApproval = false, Floor = 2, Capacity = 5 };
            var sameRoom = ExistingEvent(Today.AddDays(1), Today.AddDays(3));
            var excluded = ExistingEvent(Today.AddDays(1), Today.AddDays(3));
            var differentRoom = new Event { Name = "Inna sala", StartDate = Today.AddDays(1), EndDate = Today.AddDays(3), IsClosed = true, Rooms = [otherRoom] };

            var result = _sut.GetConflictingEvents([_room.Id], Today.AddDays(2), Today.AddDays(2), [sameRoom, excluded, differentRoom], excludeEventId: excluded.Id);

            result.Should().ContainSingle().Which.Should().BeSameAs(sameRoom);
        }

        #endregion
        #region DeleteAsync

        [Fact]
        public async Task DeleteAsync_WhenEventDoesNotExist_ReturnsNotFound()
        {
            _eventsMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Event?)null);

            var result = await _sut.DeleteAsync(Guid.NewGuid());

            result.Error!.ErrorType.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task DeleteAsync_WhenReservationsWouldConflictAndNoForce_ReturnsConflictError()
        {
            var ev = ExistingEvent(Today.AddDays(1), Today.AddDays(1));
            _eventsMock.Setup(x => x.GetByIdAsync(ev.Id)).ReturnsAsync(ev);
            _availabilitiesMock.Setup(x => x.GetByRoomIdsAsync(It.IsAny<IReadOnlyList<Guid>>())).ReturnsAsync([]);
            _availabilityServiceMock.Setup(x => x.GetConflictingReservationsForRoomsAsync(It.IsAny<IReadOnlyList<Guid>>(), It.IsAny<IReadOnlyList<Availability>>(), It.IsAny<IReadOnlyList<Event>>()))
                                    .ReturnsAsync([ReservationOn(Today.AddDays(1))]);

            var result = await _sut.DeleteAsync(ev.Id);

            result.Error.Should().BeOfType<ConflictError<ConflictingReservationModel>>();
            _eventsMock.Verify(x => x.Remove(It.IsAny<Event>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenForced_RemovesEventAndSaves()
        {
            var ev = ExistingEvent(Today.AddDays(1), Today.AddDays(1));
            _eventsMock.Setup(x => x.GetByIdAsync(ev.Id)).ReturnsAsync(ev);
            _availabilitiesMock.Setup(x => x.GetByRoomIdsAsync(It.IsAny<IReadOnlyList<Guid>>())).ReturnsAsync([]);
            _availabilityServiceMock.Setup(x => x.GetConflictingReservationsForRoomsAsync(It.IsAny<IReadOnlyList<Guid>>(), It.IsAny<IReadOnlyList<Availability>>(), It.IsAny<IReadOnlyList<Event>>()))
                                    .ReturnsAsync([ReservationOn(Today.AddDays(1))]);

            var result = await _sut.DeleteAsync(ev.Id, force: true);

            result.IsSuccess.Should().BeTrue();
            _eventsMock.Verify(x => x.Remove(ev), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion
        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_WhenDataIsValid_UpdatesEventAndSaves()
        {
            var ev = ExistingEvent(Today.AddDays(1), Today.AddDays(1));
            _eventsMock.Setup(x => x.GetByIdAsync(ev.Id)).ReturnsAsync(ev);
            _eventsMock.Setup(x => x.GetActiveByRoomIdsAsync(It.IsAny<IReadOnlyList<Guid>>())).ReturnsAsync([ev]);

            var request = new EventModel("Nowa nazwa", Today.AddDays(2), Today.AddDays(3), false, new TimeOnly(9, 0), new TimeOnly(12, 0));
            var result = await _sut.UpdateAsync(ev.Id, [_room.Id], request);

            result.IsSuccess.Should().BeTrue();
            ev.Name.Should().Be("Nowa nazwa");
            ev.IsClosed.Should().BeFalse();
            ev.StartTime.Should().Be(new TimeOnly(9, 0));
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion
    }
}
