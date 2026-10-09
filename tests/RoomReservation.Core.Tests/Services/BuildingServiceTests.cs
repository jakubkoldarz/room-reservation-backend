using FluentAssertions;
using Moq;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Models.Availability;
using RoomReservation.Core.Results.Common;
using RoomReservation.Core.Services;

namespace RoomReservation.Core.Tests.Services
{
    public class BuildingServiceTests
    {
        private readonly Mock<IBuildingRepository> _buildingsMock = new();
        private readonly Mock<IRoomRepository> _roomsMock = new();
        private readonly Mock<IAvailabilityService> _availabilityServiceMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

        private readonly BuildingService _sut;

        public BuildingServiceTests()
        {
            _sut = new(_buildingsMock.Object, _roomsMock.Object, _availabilityServiceMock.Object, _unitOfWorkMock.Object);

            _availabilityServiceMock.Setup(x => x.AreAvailabilitiesValid(It.IsAny<IReadOnlyList<Availability>>(), It.IsAny<Guid?>()))
                                    .ReturnsAsync(Result.Success());
        }

        private static BuildingModel CreateModel(string name = "Budynek A") => new(
            Name: name,
            Identifier: "A",
            Street: "Długa 1",
            City: "Kraków",
            PostalCode: "30-001",
            FloorsCount: 3,
            Availabilities: [new AvailabilityModel(DayOfWeek.Monday, new TimeOnly(8, 0), new TimeOnly(20, 0))]);

        private static Building CreateBuilding(string name = "Budynek A") => new()
        {
            Name = name,
            Street = "Krótka 2",
            City = "Warszawa",
            PostalCode = "00-001",
            FloorsCount = 1
        };

        #region CreateAsync

        [Fact]
        public async Task CreateAsync_WhenNameIsTaken_ReturnsConflict()
        {
            _buildingsMock.Setup(x => x.ExistsByNameAsync("Budynek A")).ReturnsAsync(true);

            var result = await _sut.CreateAsync(CreateModel());

            result.IsSuccess.Should().BeFalse();
            result.Error!.ErrorType.Should().Be(ErrorType.Conflict);
            _buildingsMock.Verify(x => x.Add(It.IsAny<Building>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenAvailabilitiesAreInvalid_ReturnsValidationError()
        {
            _availabilityServiceMock.Setup(x => x.AreAvailabilitiesValid(It.IsAny<IReadOnlyList<Availability>>(), It.IsAny<Guid?>()))
                                    .ReturnsAsync(Result.Failure(new Error("Invalid availability", ErrorType.BadRequest)));

            var result = await _sut.CreateAsync(CreateModel());

            result.IsSuccess.Should().BeFalse();
            result.Error!.ErrorMessage.Should().Be("Invalid availability");
            _buildingsMock.Verify(x => x.Add(It.IsAny<Building>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenDataIsValid_AddsBuildingWithAvailabilities()
        {
            var result = await _sut.CreateAsync(CreateModel());

            result.IsSuccess.Should().BeTrue();
            result.Value!.Name.Should().Be("Budynek A");
            result.Value.Availabilities.Should().ContainSingle(a => a.DayOfWeek == DayOfWeek.Monday);
            _buildingsMock.Verify(x => x.Add(result.Value), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion
        #region DeleteAsync

        [Fact]
        public async Task DeleteAsync_WhenBuildingDoesNotExist_ReturnsNotFound()
        {
            _buildingsMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Building?)null);

            var result = await _sut.DeleteAsync(Guid.NewGuid());

            result.Error!.ErrorType.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task DeleteAsync_WhenBuildingHasRooms_ReturnsConflict()
        {
            var building = CreateBuilding();
            _buildingsMock.Setup(x => x.GetByIdAsync(building.Id)).ReturnsAsync(building);
            _roomsMock.Setup(x => x.GetFilteredAsync(It.IsAny<RoomFilter>()))
                      .ReturnsAsync(new PagedList<Room>([], 1, 1, 1));

            var result = await _sut.DeleteAsync(building.Id);

            result.Error!.ErrorType.Should().Be(ErrorType.Conflict);
            _buildingsMock.Verify(x => x.Remove(It.IsAny<Building>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenBuildingHasNoRooms_RemovesAndSaves()
        {
            var building = CreateBuilding();
            _buildingsMock.Setup(x => x.GetByIdAsync(building.Id)).ReturnsAsync(building);
            _roomsMock.Setup(x => x.GetFilteredAsync(It.IsAny<RoomFilter>()))
                      .ReturnsAsync(new PagedList<Room>([], 0, 1, 1));

            var result = await _sut.DeleteAsync(building.Id);

            result.IsSuccess.Should().BeTrue();
            _buildingsMock.Verify(x => x.Remove(building), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion
        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_WhenNameBelongsToOtherBuilding_ReturnsConflict()
        {
            var building = CreateBuilding();
            _buildingsMock.Setup(x => x.GetByIdAsync(building.Id)).ReturnsAsync(building);
            _buildingsMock.Setup(x => x.GetByNameAsync("Budynek B")).ReturnsAsync(CreateBuilding("Budynek B"));

            var result = await _sut.UpdateAsync(building.Id, CreateModel("Budynek B"));

            result.Error!.ErrorType.Should().Be(ErrorType.Conflict);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenAvailabilityReplacementFails_DoesNotSave()
        {
            var building = CreateBuilding();
            _buildingsMock.Setup(x => x.GetByIdAsync(building.Id)).ReturnsAsync(building);
            _availabilityServiceMock.Setup(x => x.ReplaceIfValidForBuildingAsync(building, It.IsAny<IReadOnlyList<AvailabilityModel>>()))
                                    .ReturnsAsync(ResultT<IReadOnlyList<Availability>>.Failure(new Error("Rooms conflict", ErrorType.Conflict)));

            var result = await _sut.UpdateAsync(building.Id, CreateModel());

            result.IsSuccess.Should().BeFalse();
            result.Error!.ErrorMessage.Should().Be("Rooms conflict");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenDataIsValid_UpdatesFieldsAndSaves()
        {
            var building = CreateBuilding();
            _buildingsMock.Setup(x => x.GetByIdAsync(building.Id)).ReturnsAsync(building);
            _availabilityServiceMock.Setup(x => x.ReplaceIfValidForBuildingAsync(building, It.IsAny<IReadOnlyList<AvailabilityModel>>()))
                                    .ReturnsAsync(ResultT<IReadOnlyList<Availability>>.Success([]));

            var result = await _sut.UpdateAsync(building.Id, CreateModel("Nowa nazwa"));

            result.IsSuccess.Should().BeTrue();
            building.Name.Should().Be("Nowa nazwa");
            building.City.Should().Be("Kraków");
            building.FloorsCount.Should().Be(3);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion
    }
}
