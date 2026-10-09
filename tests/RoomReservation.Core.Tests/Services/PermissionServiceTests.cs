using FluentAssertions;
using Moq;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Services;
using RoomReservation.Core.Tests.TestHelpers;

namespace RoomReservation.Core.Tests.Services
{
    public class PermissionServiceTests
    {
        private readonly Mock<IUserRepository> _usersMock = new();
        private readonly Mock<IPermissionRepository> _permissionsMock = new();

        private readonly PermissionService _sut;

        public PermissionServiceTests()
        {
            _sut = new(_usersMock.Object, _permissionsMock.Object);
        }

        [Fact]
        public async Task GetUserPermissionsAsync_WhenUserDoesNotExist_ReturnsNotFound()
        {
            _usersMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

            var result = await _sut.GetUserPermissionsAsync(Guid.NewGuid());

            result.Error!.ErrorType.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task GetUserPermissionsAsync_WhenSuperAdmin_ReturnsAllPermissions()
        {
            var user = UserFaker.Create();
            user.Role = new Role { Name = "SuperAdmin", IsSuperAdmin = true };
            _usersMock.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _permissionsMock.Setup(x => x.GetAllAsync()).ReturnsAsync(["a", "b", "c"]);

            var result = await _sut.GetUserPermissionsAsync(user.Id);

            result.Value.Should().BeEquivalentTo(new[] { "a", "b", "c" });
            _permissionsMock.Verify(x => x.GetUserPermissionsAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact]
        public async Task GetUserPermissionsAsync_WhenRegularUser_ReturnsRolePermissions()
        {
            var user = UserFaker.Create();
            user.Role = new Role { Name = "User" };
            _usersMock.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _permissionsMock.Setup(x => x.GetUserPermissionsAsync(user.Id)).ReturnsAsync(["a"]);

            var result = await _sut.GetUserPermissionsAsync(user.Id);

            result.Value.Should().BeEquivalentTo(new[] { "a" });
        }

        [Fact]
        public async Task GetUserAccessAsync_ReturnsAccessFromRepository()
        {
            var userId = Guid.NewGuid();
            var access = new UserAccessModel(true, false, new HashSet<string> { "a" });
            _permissionsMock.Setup(x => x.GetUserAccessAsync(userId)).ReturnsAsync(access);

            var result = await _sut.GetUserAccessAsync(userId);

            result.Should().BeSameAs(access);
        }
    }
}
