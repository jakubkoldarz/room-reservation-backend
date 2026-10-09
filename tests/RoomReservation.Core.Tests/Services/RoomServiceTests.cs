using FluentAssertions;
using Moq;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models.Availability;
using RoomReservation.Core.Models.Rooms;
using RoomReservation.Core.Results.Common;
using RoomReservation.Core.Services;

namespace RoomReservation.Core.Tests.Services
{
    public class RoomServiceTests
    {
        private readonly Mock<IRoomRepository> _roomsMock = new();
        private readonly Mock<IAvailabilityService> _availabilityServiceMock = new();
        private readonly Mock<IReservationService> _reservationServiceMock = new();
        private readonly Mock<IReservationRepository> _reservationsMock = new();
        private readonly Mock<IBuildingRepository> _buildingsMock = new();
        private readonly Mock<IEquipmentRepository> _equipmentMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

        private readonly RoomService _sut;

        private readonly Building _building = new() { Name = "A", Street = "s", City = "c", PostalCode = "p", FloorsCount = 2 };

        public RoomServiceTests()
        {
            _sut = new(_roomsMock.Object,
                       _availabilityServiceMock.Object,
                       _reservationServiceMock.Object,
                       _reservationsMock.Object,
                       _buildingsMock.Object,
                       _equipmentMock.Object,
                       _unitOfWorkMock.Object);

            _equipmentMock.Setup(x => x.AllExistAsync(It.IsAny<IReadOnlyList<Guid>>())).ReturnsAsync(true);
            _buildingsMock.Setup(x => x.GetByIdAsync(_building.Id)).ReturnsAsync(_building);
            _availabilityServiceMock.Setup(x => x.AreAvailabilitiesValid(It.IsAny<IReadOnlyList<Availability>>(), It.IsAny<Guid?>()))
                                    .ReturnsAsync(Result.Success());
            _reservationServiceMock.Setup(x => x.BulkForceCancelAsync(It.IsAny<IReadOnlyList<Reservation>>(), It.IsAny<string?>(), It.IsAny<Guid?>()))
                                   .ReturnsAsync(Result.Success());
        }

        private RoomModel CreateModel(IReadOnlyList<Guid>? equipmentIds = null, Guid? buildingId = null) => new(
            Identifier: "101",
            RequiresApproval: false,
            BuildingId: buildingId ?? _building.Id,
            Floor: 1,
            Capacity: 30,
            EquipmentIds: equipmentIds ?? [],
            Availabilities: [new AvailabilityModel(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(16, 0))]);

        private Room CreateRoom() => new()
        {
            Identifier = "100",
            RequiresApproval = true,
            BuildingId = _building.Id,
            Building = _building,
            Floor = 0,
            Capacity = 10
        };

        #region CreateAsync

        [Fact]
        public async Task CreateAsync_WhenEquipmentIdsAreDuplicated_ReturnsBadRequest()
        {
            var equipmentId = Guid.NewGuid();

            var result = await _sut.CreateAsync(CreateModel(equipmentIds: [equipmentId, equipmentId]));

            result.Error!.ErrorType.Should().Be(ErrorType.BadRequest);
            _roomsMock.Verify(x => x.Add(It.IsAny<Room>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenEquipmentDoesNotExist_ReturnsBadRequest()
        {
            _equipmentMock.Setup(x => x.AllExistAsync(It.IsAny<IReadOnlyList<Guid>>())).ReturnsAsync(false);

            var result = await _sut.CreateAsync(CreateModel(equipmentIds: [Guid.NewGuid()]));

            result.Error!.ErrorType.Should().Be(ErrorType.BadRequest);
        }

        [Fact]
        public async Task CreateAsync_WhenBuildingDoesNotExist_ReturnsNotFound()
        {
            var result = await _sut.CreateAsync(CreateModel(buildingId: Guid.NewGuid()));

            result.Error!.ErrorType.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task CreateAsync_WhenIdentifierIsTakenInBuilding_ReturnsConflict()
        {
            _roomsMock.Setup(x => x.ExistsByIdentifierAsync(_building.Id, "101", null)).ReturnsAsync(true);

            var result = await _sut.CreateAsync(CreateModel());

            result.Error!.ErrorType.Should().Be(ErrorType.Conflict);
        }

        [Fact]
        public async Task CreateAsync_WhenDataIsValid_AddsRoomAndReturnsReloadedEntity()
        {
            var equipmentId = Guid.NewGuid();
            Room? addedRoom = null;
            _roomsMock.Setup(x => x.Add(It.IsAny<Room>())).Callback<Room>(r => addedRoom = r);
            _roomsMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(() => addedRoom);

            var result = await _sut.CreateAsync(CreateModel(equipmentIds: [equipmentId]));

            result.IsSuccess.Should().BeTrue();
            addedRoom.Should().NotBeNull();
            addedRoom!.RoomEquipment.Should().ContainSingle(re => re.EquipmentId == equipmentId);
            addedRoom.Availabilities.Should().HaveCount(1);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion
        #region GetAllAsync

        [Fact]
        public async Task GetAllAsync_WhenOnlyStartTimeIsProvided_ReturnsBadRequest()
        {
            var result = await _sut.GetAllAsync(new RoomFilter { StartTime = new TimeOnly(10, 0) });

            result.Error!.ErrorType.Should().Be(ErrorType.BadRequest);
        }

        [Fact]
        public async Task GetAllAsync_WhenStartTimeIsNotBeforeEndTime_ReturnsBadRequest()
        {
            var result = await _sut.GetAllAsync(new RoomFilter { StartTime = new TimeOnly(12, 0), EndTime = new TimeOnly(12, 0) });

            result.Error!.ErrorType.Should().Be(ErrorType.BadRequest);
        }

        #endregion
        #region DeleteAsync

        [Fact]
        public async Task DeleteAsync_WhenRoomHasActiveReservationsAndNoForce_ReturnsConflict()
        {
            var room = CreateRoom();
            _roomsMock.Setup(x => x.GetByIdAsync(room.Id)).ReturnsAsync(room);
            IReadOnlyList<Reservation> reservations = [new Reservation { RoomId = room.Id, Date = new DateOnly(2030, 1, 1), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(9, 0) }];
            _reservationsMock.Setup(x => x.GetActiveFutureByRoomAsync(room.Id)).ReturnsAsync(reservations);

            var result = await _sut.DeleteAsync(room.Id);

            result.Error!.ErrorType.Should().Be(ErrorType.Conflict);
            _roomsMock.Verify(x => x.Remove(It.IsAny<Room>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenForced_CancelsReservationsAndRemovesRoom()
        {
            var room = CreateRoom();
            IReadOnlyList<Reservation> reservations = [new Reservation { RoomId = room.Id, Date = new DateOnly(2030, 1, 1), StartTime = new TimeOnly(8, 0), EndTime = new TimeOnly(9, 0) }];
            _roomsMock.Setup(x => x.GetByIdAsync(room.Id)).ReturnsAsync(room);
            _reservationsMock.Setup(x => x.GetActiveFutureByRoomAsync(room.Id)).ReturnsAsync(reservations);

            var result = await _sut.DeleteAsync(room.Id, force: true);

            result.IsSuccess.Should().BeTrue();
            _reservationServiceMock.Verify(x => x.BulkForceCancelAsync(reservations, It.IsAny<string?>(), It.IsAny<Guid?>()), Times.Once);
            _roomsMock.Verify(x => x.Remove(room), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion
        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_ReplacesOnlyChangedEquipment()
        {
            var room = CreateRoom();
            var keptId = Guid.NewGuid();
            var removedId = Guid.NewGuid();
            var addedId = Guid.NewGuid();
            var keptEntry = new RoomEquipment { RoomId = room.Id, EquipmentId = keptId };
            room.RoomEquipment = [keptEntry, new RoomEquipment { RoomId = room.Id, EquipmentId = removedId }];

            _roomsMock.Setup(x => x.GetByIdAsync(room.Id)).ReturnsAsync(room);
            _availabilityServiceMock.Setup(x => x.ReplaceIfValidForRoomAsync(room, It.IsAny<IReadOnlyList<AvailabilityModel>>(), false))
                                    .ReturnsAsync(ResultT<IReadOnlyList<Reservation>>.Success([]));

            var result = await _sut.UpdateAsync(room.Id, CreateModel(equipmentIds: [keptId, addedId]));

            result.IsSuccess.Should().BeTrue();
            room.RoomEquipment.Select(re => re.EquipmentId).Should().BeEquivalentTo(new[] { keptId, addedId });
            room.RoomEquipment.Should().Contain(keptEntry);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ValidatesAvailabilitiesAgainstNewBuilding()
        {
            var room = CreateRoom();
            var newBuilding = new Building { Name = "B", Street = "s", City = "c", PostalCode = "p", FloorsCount = 1 };
            Guid? buildingIdDuringValidation = null;

            _roomsMock.Setup(x => x.GetByIdAsync(room.Id)).ReturnsAsync(room);
            _buildingsMock.Setup(x => x.GetByIdAsync(newBuilding.Id)).ReturnsAsync(newBuilding);
            _availabilityServiceMock.Setup(x => x.ReplaceIfValidForRoomAsync(room, It.IsAny<IReadOnlyList<AvailabilityModel>>(), false))
                                    .Callback<Room, IReadOnlyList<AvailabilityModel>, bool>((r, _, _) => buildingIdDuringValidation = r.BuildingId)
                                    .ReturnsAsync(ResultT<IReadOnlyList<Reservation>>.Success([]));

            await _sut.UpdateAsync(room.Id, CreateModel(buildingId: newBuilding.Id));

            buildingIdDuringValidation.Should().Be(newBuilding.Id);
        }

        [Fact]
        public async Task UpdateAsync_WhenAvailabilityConflictsAndNoForce_DoesNotSave()
        {
            var room = CreateRoom();
            _roomsMock.Setup(x => x.GetByIdAsync(room.Id)).ReturnsAsync(room);
            _availabilityServiceMock.Setup(x => x.ReplaceIfValidForRoomAsync(room, It.IsAny<IReadOnlyList<AvailabilityModel>>(), false))
                                    .ReturnsAsync(ResultT<IReadOnlyList<Reservation>>.Failure(new Error("Conflicting reservations found", ErrorType.Conflict)));

            var result = await _sut.UpdateAsync(room.Id, CreateModel());

            result.Error!.ErrorType.Should().Be(ErrorType.Conflict);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        #endregion
    }
}
