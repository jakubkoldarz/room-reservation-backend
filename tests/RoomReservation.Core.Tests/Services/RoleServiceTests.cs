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
    public class RoleServiceTests
    {
        private readonly Mock<IRoleRepository> _rolesMock = new();
        private readonly Mock<IUserRepository> _usersMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

        private readonly RoleService _sut;

        public RoleServiceTests()
        {
            _sut = new(_rolesMock.Object, _usersMock.Object, _unitOfWorkMock.Object);
        }

        private static RoleModel CreateModel(bool isDefault = false, params Guid[] permissionIds)
            => new("Pracownik", "opis", isDefault, false, permissionIds);

        #region AssignRoleAsync

        [Fact]
        public async Task AssignRoleAsync_WhenAssigningToYourself_ReturnsBadRequest()
        {
            var userId = Guid.NewGuid();

            var result = await _sut.AssignRoleAsync(Guid.NewGuid(), userId, requestingUserId: userId);

            result.Error!.ErrorType.Should().Be(ErrorType.BadRequest);
        }

        [Fact]
        public async Task AssignRoleAsync_WhenRoleDoesNotExist_ReturnsNotFound()
        {
            var user = UserFaker.Create();
            _usersMock.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _rolesMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Role?)null);

            var result = await _sut.AssignRoleAsync(Guid.NewGuid(), user.Id, Guid.NewGuid());

            result.Error!.ErrorType.Should().Be(ErrorType.NotFound);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AssignRoleAsync_WhenDataIsValid_ChangesRoleAndSaves()
        {
            var user = UserFaker.Create();
            var role = new Role { Name = "Admin" };
            _usersMock.Setup(x => x.GetByIdAsync(user.Id)).ReturnsAsync(user);
            _rolesMock.Setup(x => x.GetByIdAsync(role.Id)).ReturnsAsync(role);

            var result = await _sut.AssignRoleAsync(role.Id, user.Id, Guid.NewGuid());

            result.IsSuccess.Should().BeTrue();
            user.RoleId.Should().Be(role.Id);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion
        #region CreateAsync

        [Fact]
        public async Task CreateAsync_WhenDefaultRoleExistsAndNoForce_ReturnsConflict()
        {
            _rolesMock.Setup(x => x.GetDefaultRoleAsync()).ReturnsAsync(new Role { Name = "User", IsDefault = true });

            var result = await _sut.CreateAsync(CreateModel(isDefault: true));

            result.Error!.ErrorType.Should().Be(ErrorType.Conflict);
            _rolesMock.Verify(x => x.Add(It.IsAny<Role>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_WhenDefaultRoleExistsAndForce_ReplacesDefaultRole()
        {
            var previousDefault = new Role { Name = "User", IsDefault = true };
            _rolesMock.Setup(x => x.GetDefaultRoleAsync()).ReturnsAsync(previousDefault);

            var result = await _sut.CreateAsync(CreateModel(isDefault: true), force: true);

            result.IsSuccess.Should().BeTrue();
            result.Value!.IsDefault.Should().BeTrue();
            previousDefault.IsDefault.Should().BeFalse();
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_AssignsPermissionsToNewRole()
        {
            var permissionA = Guid.NewGuid();
            var permissionB = Guid.NewGuid();

            var result = await _sut.CreateAsync(CreateModel(false, permissionA, permissionB));

            result.IsSuccess.Should().BeTrue();
            result.Value!.RolePermissions.Select(rp => rp.PermissionId).Should().BeEquivalentTo(new[] { permissionA, permissionB });
            result.Value.RolePermissions.Should().OnlyContain(rp => rp.RoleId == result.Value.Id);
            _rolesMock.Verify(x => x.Add(result.Value), Times.Once);
        }

        #endregion
        #region DeleteAsync

        [Fact]
        public async Task DeleteAsync_WhenRoleHasUsers_ReturnsConflict()
        {
            var role = new Role { Name = "User" };
            _rolesMock.Setup(x => x.GetByIdAsync(role.Id)).ReturnsAsync(role);
            _usersMock.Setup(x => x.GetFilteredAsync(It.IsAny<UserFilter>()))
                      .ReturnsAsync(new PagedList<User>([], 3, 1, 1));

            var result = await _sut.DeleteAsync(role.Id);

            result.Error!.ErrorType.Should().Be(ErrorType.Conflict);
            _rolesMock.Verify(x => x.Remove(It.IsAny<Role>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAsync_WhenRoleHasNoUsers_RemovesAndSaves()
        {
            var role = new Role { Name = "User" };
            _rolesMock.Setup(x => x.GetByIdAsync(role.Id)).ReturnsAsync(role);
            _usersMock.Setup(x => x.GetFilteredAsync(It.IsAny<UserFilter>()))
                      .ReturnsAsync(new PagedList<User>([], 0, 1, 1));

            var result = await _sut.DeleteAsync(role.Id);

            result.IsSuccess.Should().BeTrue();
            _rolesMock.Verify(x => x.Remove(role), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion
        #region UpdateAsync

        [Fact]
        public async Task UpdateAsync_WhenRoleDoesNotExist_ReturnsNotFound()
        {
            _rolesMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Role?)null);

            var result = await _sut.UpdateAsync(Guid.NewGuid(), CreateModel());

            result.Error!.ErrorType.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task UpdateAsync_ReplacesOnlyChangedPermissions()
        {
            var role = new Role { Name = "Stara" };
            var keptId = Guid.NewGuid();
            var removedId = Guid.NewGuid();
            var addedId = Guid.NewGuid();
            var keptEntry = new RolePermissions { RoleId = role.Id, PermissionId = keptId };
            role.RolePermissions = [keptEntry, new RolePermissions { RoleId = role.Id, PermissionId = removedId }];
            _rolesMock.Setup(x => x.GetByIdAsync(role.Id)).ReturnsAsync(role);

            var result = await _sut.UpdateAsync(role.Id, CreateModel(false, keptId, addedId));

            result.IsSuccess.Should().BeTrue();
            role.Name.Should().Be("Pracownik");
            role.RolePermissions.Select(rp => rp.PermissionId).Should().BeEquivalentTo(new[] { keptId, addedId });
            role.RolePermissions.Should().Contain(keptEntry);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WhenRoleIsAlreadyDefault_DoesNotReportConflict()
        {
            var role = new Role { Name = "User", IsDefault = true };
            _rolesMock.Setup(x => x.GetByIdAsync(role.Id)).ReturnsAsync(role);
            _rolesMock.Setup(x => x.GetDefaultRoleAsync()).ReturnsAsync(role);

            var result = await _sut.UpdateAsync(role.Id, CreateModel(isDefault: true));

            result.IsSuccess.Should().BeTrue();
            role.IsDefault.Should().BeTrue();
        }

        #endregion
    }
}
