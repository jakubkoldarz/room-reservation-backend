using FluentAssertions;
using Moq;
using RoomReservation.Core.Emails;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Results.Common;
using RoomReservation.Core.Services;
using RoomReservation.Core.Tests.TestHelpers;

namespace RoomReservation.Core.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _usersMock = new();
        private readonly Mock<ITokenProvider> _tokenProviderMock = new();
        private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock = new();
        private readonly Mock<IVerificationCodeService> _verificationCodeServiceMock = new();
        private readonly Mock<IRoleRepository> _rolesMock = new();
        private readonly Mock<IEmailService> _emailServiceMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

        private readonly AuthService _sut;
        public AuthServiceTests()
        {
            _sut = new(_usersMock.Object,
                       _tokenProviderMock.Object,
                       _refreshTokenServiceMock.Object,
                       _verificationCodeServiceMock.Object,
                       _rolesMock.Object,
                       _emailServiceMock.Object,
                       _unitOfWorkMock.Object);
        }

        #region LoginAsync

        [Fact]
        public async Task LoginAsync_WhenPasswordIsWrong_ReturnsFailure()
        {
            var testUser = UserFaker.Create(email: "jan@test.com", password: "correct_password");

            _usersMock.Setup(repo => repo.GetByEmailAsync("jan@test.com"))
                      .ReturnsAsync(testUser);

            var result = await _sut.LoginAsync("jan@test.com", "wrong_password");

            result.IsSuccess.Should().BeFalse();
            result.Error!.ErrorMessage.Should().Be("Invalid credentials");

            _refreshTokenServiceMock.Verify(x => x.CreateToken(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<string?>()), Times.Never);
            _tokenProviderMock.Verify(x => x.GenerateJwtToken(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_WhenUserDoesNotExist_ReturnsFailure()
        {
            _usersMock.Setup(repo => repo.GetByEmailAsync("fake@test.com"))
                      .ReturnsAsync((User?)null);

            var result = await _sut.LoginAsync("fake@test.com", "any_password");

            result.IsSuccess.Should().BeFalse();
            result.Error!.ErrorMessage.Should().Be("Invalid credentials");

            _refreshTokenServiceMock.Verify(x => x.CreateToken(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<string?>()), Times.Never);
            _tokenProviderMock.Verify(x => x.GenerateJwtToken(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_WhenCredentialsAreValid_ReturnsSuccessWithTokens()
        {
            var testUser = UserFaker.Create(email: "jan@test.com", password: "correct_password");

            _usersMock.Setup(repo => repo.GetByEmailAsync("jan@test.com"))
                      .ReturnsAsync(testUser);
            _usersMock.Setup(repo => repo.GetByIdAsync(testUser.Id))
                      .ReturnsAsync(testUser);

            _refreshTokenServiceMock.Setup(s => s.CreateToken(testUser.Id, It.IsAny<string?>(), It.IsAny<string?>()))
                                    .Returns("fake-refresh-token");
            _refreshTokenServiceMock.Setup(s => s.DeleteExpiredAsync(testUser.Id))
                                    .ReturnsAsync(Result.Success());

            _tokenProviderMock.Setup(tp => tp.GenerateJwtToken(testUser))
                              .Returns("fake-jwt-token");

            var result = await _sut.LoginAsync("jan@test.com", "correct_password");

            result.IsSuccess.Should().BeTrue();
            result.Value.Requires2FA.Should().BeFalse();
            result.Value.JwtToken.Should().Be("fake-jwt-token");
            result.Value.RefreshToken.Should().Be("fake-refresh-token");

            _refreshTokenServiceMock.Verify(s => s.CreateToken(testUser.Id, It.IsAny<string?>(), It.IsAny<string?>()), Times.Once);
            _refreshTokenServiceMock.Verify(s => s.DeleteExpiredAsync(testUser.Id), Times.Once);
            _tokenProviderMock.Verify(tp => tp.GenerateJwtToken(testUser), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion
        #region RegisterAsync

        [Fact]
        public async Task RegisterAsync_WhenEmailAlreadyExists_ReturnsFailure()
        {
            var password = "correct_password";
            var testUser = UserFaker.Create(email: "jan@test.com", password: password);

            _usersMock.Setup(x => x.GetByEmailAsync("jan@test.com"))
                      .ReturnsAsync(testUser);

            var result = await _sut.RegisterAsync("jan@test.com", password);

            result.IsSuccess.Should().BeFalse();
            result.Error!.ErrorMessage.Should().Be("Email is already taken");

            _usersMock.Verify(x => x.Add(It.IsAny<User>()), Times.Never);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_WhenDataIsValid_CreatesUserAndReturnsTokens()
        {
            var password = "correct_password";
            var defaultRole = new Role { Name = "User", IsDefault = true };

            _usersMock.Setup(x => x.GetByEmailAsync("jan@test.com"))
                      .ReturnsAsync((User?)null);
            _rolesMock.Setup(x => x.GetDefaultRoleAsync())
                      .ReturnsAsync(defaultRole);

            _verificationCodeServiceMock.Setup(x => x.GenerateCodeAsync(It.IsAny<Guid>(), VerificationCodeType.EmailActivation))
                                        .ReturnsAsync((Guid userId, VerificationCodeType type) => ResultT<VerificationCode>.Success(new VerificationCode
                                        {
                                            UserId = userId,
                                            Type = type,
                                            Code = "123456",
                                            ExpiresAt = DateTime.UtcNow.AddMinutes(15)
                                        }));
            _refreshTokenServiceMock.Setup(x => x.CreateToken(It.IsAny<Guid>(), It.IsAny<string?>(), It.IsAny<string?>()))
                                    .Returns("fake-refresh-token");
            _tokenProviderMock.Setup(x => x.GenerateJwtToken(It.IsAny<User>()))
                              .Returns("fake-jwt-token");

            var result = await _sut.RegisterAsync("jan@test.com", password);

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(("fake-jwt-token", "fake-refresh-token"));

            _usersMock.Verify(x => x.Add(It.Is<User>(u =>
                u.Email == "jan@test.com" &&
                u.RoleId == defaultRole.Id &&
                BCrypt.Net.BCrypt.Verify(password, u.PasswordHash)
            )), Times.Once);
            _emailServiceMock.Verify(x => x.EnqueueEmail(It.Is<VerificationCodeEmail>(e => e.To == "jan@test.com")), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }
        #endregion
        #region ChangePasswordAsync

        [Fact]
        public async Task ChangePasswordAsync_WhenOldPasswordIsValid_SavesNewPasswordBeforeRevokingSessions()
        {
            var testUser = UserFaker.Create(password: "old_password");
            var calls = new List<string>();

            _usersMock.Setup(x => x.GetByIdAsync(testUser.Id))
                      .ReturnsAsync(testUser);
            _unitOfWorkMock.Setup(x => x.SaveChangesAsync())
                           .Callback(() => calls.Add("save"))
                           .Returns(Task.CompletedTask);
            _refreshTokenServiceMock.Setup(x => x.RevokeAllAsync(testUser.Id))
                                    .Callback(() => calls.Add("revoke"))
                                    .ReturnsAsync(Result.Success());

            var result = await _sut.ChangePasswordAsync(testUser.Id, "old_password", "new_password");

            result.IsSuccess.Should().BeTrue();
            BCrypt.Net.BCrypt.Verify("new_password", testUser.PasswordHash).Should().BeTrue();
            calls.Should().Equal("save", "revoke");
        }

        [Fact]
        public async Task ChangePasswordAsync_WhenOldPasswordIsWrong_DoesNotChangeAnything()
        {
            var testUser = UserFaker.Create(password: "old_password");
            var originalHash = testUser.PasswordHash;

            _usersMock.Setup(x => x.GetByIdAsync(testUser.Id))
                      .ReturnsAsync(testUser);

            var result = await _sut.ChangePasswordAsync(testUser.Id, "wrong_password", "new_password");

            result.IsSuccess.Should().BeFalse();
            testUser.PasswordHash.Should().Be(originalHash);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
            _refreshTokenServiceMock.Verify(x => x.RevokeAllAsync(It.IsAny<Guid>()), Times.Never);
        }
        #endregion
    }
}
