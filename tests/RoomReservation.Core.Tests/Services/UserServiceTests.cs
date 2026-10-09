using FluentAssertions;
using Moq;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Services;
using RoomReservation.Core.Tests.TestHelpers;

namespace RoomReservation.Core.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _usersMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

        private readonly UserService _sut;

        public UserServiceTests()
        {
            _sut = new(_usersMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task GetUserDetailsAsync_WhenUserDoesNotExist_ReturnsNotFound()
        {
            _usersMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                      .ReturnsAsync((User?)null);

            var result = await _sut.GetUserDetailsAsync(Guid.NewGuid());

            result.IsSuccess.Should().BeFalse();
            result.Error!.ErrorType.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task GetUserDetailsAsync_WhenUserExists_ReturnsUser()
        {
            var user = UserFaker.Create();
            _usersMock.Setup(x => x.GetByIdAsync(user.Id))
                      .ReturnsAsync(user);

            var result = await _sut.GetUserDetailsAsync(user.Id);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeSameAs(user);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsPageFromRepository()
        {
            var filters = new UserFilter { Page = 1, PageSize = 10 };
            var page = new PagedList<User>([UserFaker.Create()], 1, 1, 10);
            _usersMock.Setup(x => x.GetFilteredAsync(filters))
                      .ReturnsAsync(page);

            var result = await _sut.GetAllAsync(filters);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().BeSameAs(page);
        }

        [Fact]
        public async Task UpdateUserAsync_WhenUserDoesNotExist_ReturnsNotFoundAndDoesNotSave()
        {
            _usersMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>()))
                      .ReturnsAsync((User?)null);

            var result = await _sut.UpdateUserAsync(Guid.NewGuid(), "Jan", "Kowalski");

            result.IsSuccess.Should().BeFalse();
            result.Error!.ErrorType.Should().Be(ErrorType.NotFound);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UpdateUserAsync_WhenUserExists_UpdatesNamesAndCompletesProfile()
        {
            var user = UserFaker.Create(firstname: "Old", lastname: "Name");
            _usersMock.Setup(x => x.GetByIdAsync(user.Id))
                      .ReturnsAsync(user);

            var result = await _sut.UpdateUserAsync(user.Id, "Anna", "Nowak");

            result.IsSuccess.Should().BeTrue();
            user.Firstname.Should().Be("Anna");
            user.Lastname.Should().Be("Nowak");
            user.IsProfileComplete.Should().BeTrue();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
    }
}
