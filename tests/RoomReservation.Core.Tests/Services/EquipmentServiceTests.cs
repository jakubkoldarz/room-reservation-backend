using FluentAssertions;
using Moq;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Services;

namespace RoomReservation.Core.Tests.Services
{
    public class EquipmentServiceTests
    {
        private readonly Mock<IEquipmentRepository> _equipmentMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

        private readonly EquipmentService _sut;

        public EquipmentServiceTests()
        {
            _sut = new(_equipmentMock.Object, _unitOfWorkMock.Object);
        }

        #region CreateAsync

        [Fact]
        public async Task CreateAsync_WhenNameIsTaken_ReturnsConflict()
        {
            _equipmentMock.Setup(x => x.ExistsByNameAsync("Projektor"))
                          .ReturnsAsync(true);

            var result = await _sut.CreateAsync("Projektor", "projector");

            result.IsSuccess.Should().BeFalse();
            result.Error!.ErrorType.Should().Be(ErrorType.Conflict);
            _equipmentMock.Verify(x => x.Add(It.IsAny<Equipment>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenNameIsFree_AddsAndSaves()
        {
            _equipmentMock.Setup(x => x.ExistsByNameAsync("Projektor"))
                          .ReturnsAsync(false);

            var result = await _sut.CreateAsync("Projektor", "projector");

            result.IsSuccess.Should().BeTrue();
            result.Value!.Name.Should().Be("Projektor");
            result.Value.Icon.Should().Be("projector");
            _equipmentMock.Verify(x => x.Add(It.Is<Equipment>(e => e.Name == "Projektor")), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion
        #region DeleteAsync

        [Fact]
        public async Task DeleteAsync_WhenEquipmentDoesNotExist_ReturnsNotFound()
        {
            _equipmentMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                          .ReturnsAsync((Equipment?)null);

            var result = await _sut.DeleteAsync(Guid.NewGuid());

            result.IsSuccess.Should().BeFalse();
            result.Error!.ErrorType.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task DeleteAsync_WhenEquipmentExists_RemovesAndSaves()
        {
            var equipment = new Equipment { Name = "Tablica", Icon = "board" };
            _equipmentMock.Setup(x => x.GetByIdAsync(equipment.Id))
                          .ReturnsAsync(equipment);

            var result = await _sut.DeleteAsync(equipment.Id);

            result.IsSuccess.Should().BeTrue();
            _equipmentMock.Verify(x => x.Remove(equipment), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion
        #region GetByIdAsync

        [Fact]
        public async Task GetByIdAsync_WhenEquipmentDoesNotExist_ReturnsNotFound()
        {
            _equipmentMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                          .ReturnsAsync((Equipment?)null);

            var result = await _sut.GetByIdAsync(Guid.NewGuid());

            result.IsSuccess.Should().BeFalse();
            result.Error!.ErrorType.Should().Be(ErrorType.NotFound);
        }

        #endregion
        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_WhenNameBelongsToOtherEquipment_ReturnsConflict()
        {
            var equipment = new Equipment { Name = "Tablica", Icon = "board" };
            var other = new Equipment { Name = "Projektor", Icon = "projector" };
            _equipmentMock.Setup(x => x.GetByIdAsync(equipment.Id)).ReturnsAsync(equipment);
            _equipmentMock.Setup(x => x.GetByNameAsync("Projektor")).ReturnsAsync(other);

            var result = await _sut.UpdateAsync(equipment.Id, "Projektor", "icon");

            result.IsSuccess.Should().BeFalse();
            result.Error!.ErrorType.Should().Be(ErrorType.Conflict);
            equipment.Name.Should().Be("Tablica");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WhenNameIsUnchanged_UpdatesIcon()
        {
            var equipment = new Equipment { Name = "Tablica", Icon = "board" };
            _equipmentMock.Setup(x => x.GetByIdAsync(equipment.Id)).ReturnsAsync(equipment);
            _equipmentMock.Setup(x => x.GetByNameAsync("Tablica")).ReturnsAsync(equipment);

            var result = await _sut.UpdateAsync(equipment.Id, "Tablica", "whiteboard");

            result.IsSuccess.Should().BeTrue();
            equipment.Icon.Should().Be("whiteboard");
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion
    }
}
