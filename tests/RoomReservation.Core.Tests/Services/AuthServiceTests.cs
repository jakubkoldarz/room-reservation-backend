using FluentAssertions;
using Moq;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Results;
using RoomReservation.Core.Services;
using RoomReservation.Core.Tests.TestHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _usersMock = new();
        private readonly Mock<ITokenProvider> _tokenProviderMock = new();
        private readonly Mock<IRefreshTokenService> _refreshTokenServiceMock = new();

        private readonly AuthService _sut;
        public AuthServiceTests()
        {
            _sut = new(_usersMock.Object,
                       _tokenProviderMock.Object,
                       _refreshTokenServiceMock.Object);
        }

        #region LoginAsync

        [Fact]
        public async Task LoginAsync_WhenPasswordIsWrong_ReturnsFailure()
        {
            var testUser = UserFaker.Create(email: "jan@test.com", password: "correct_password");

            _usersMock.Setup(repo => repo.GetUserByEmailAsync("jan@test.com"))
                      .ReturnsAsync(testUser);

            var result = await _sut.LoginAsync("jan@test.com", "wrong_password");

            result.IsSuccess.Should().BeFalse();
            result.Error!.ErrorMessage.Should().Be("Invalid credentials");

            _refreshTokenServiceMock.Verify(x => x.CreateTokenAsync(It.IsAny<Guid>()), Times.Never);
            _tokenProviderMock.Verify(x => x.GenerateJwtToken(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_WhenUserDoesNotExist_ReturnsFailure()
        {
            _usersMock.Setup(repo => repo.GetUserByEmailAsync("fake@test.com"))
                      .ReturnsAsync((User?)null);

            var result = await _sut.LoginAsync("fake@test.com", "any_password");

            result.IsSuccess.Should().BeFalse();
            result.Error!.ErrorMessage.Should().Be("Invalid credentials");

            _refreshTokenServiceMock.Verify(x => x.CreateTokenAsync(It.IsAny<Guid>()), Times.Never);
            _tokenProviderMock.Verify(x => x.GenerateJwtToken(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task LoginAsync_WhenCredentialsAreValid_ReturnsSuccessWithToken()
        {
            var testUser = UserFaker.Create(email: "jan@test.com", password: "correct_password");

            _usersMock.Setup(repo => repo.GetUserByEmailAsync("jan@test.com"))
                      .ReturnsAsync(testUser);

            _refreshTokenServiceMock.Setup(repo => repo.CreateTokenAsync(testUser.Id))
                                    .ReturnsAsync(ResultT<string>.Success("fake-refresh-token"));

            _refreshTokenServiceMock.Setup(s => s.DeleteExpiredAsync(testUser.Id))
                                    .ReturnsAsync(Result.Success());

            _tokenProviderMock.Setup(tp => tp.GenerateJwtToken(testUser))
                              .Returns("fake-jwt-token");

            var result = await _sut.LoginAsync("jan@test.com", "correct_password");

            result.IsSuccess.Should().BeTrue();
            result.Value.jwtToken.Should().Be("fake-jwt-token");
            result.Value.refreshToken.Should().Be("fake-refresh-token");

            _refreshTokenServiceMock.Verify(s => s.CreateTokenAsync(testUser.Id),Times.Once);
            _refreshTokenServiceMock.Verify(s => s.DeleteExpiredAsync(testUser.Id),Times.Once);
            _tokenProviderMock.Verify(tp => tp.GenerateJwtToken(testUser),Times.Once);
        }

        #endregion
        #region RegisterAsync
        
        [Fact]
        public async Task RegisterAsync_WhenEmailAlreadyExists_ReturnsFailure()
        {
            var password = "correct_password";
            var testUser = UserFaker.Create(email: "jan@test.com", password: password);

            _usersMock.Setup(x => x.GetUserByEmailAsync("jan@test.com"))
                      .ReturnsAsync(testUser);

            var result = await _sut.RegisterAsync("jan@test.com", password, "Jan", "Kowalski");

            result.IsSuccess.Should().BeFalse();
            result.Error!.ErrorMessage.Should().Be("Email is already taken");

            _usersMock.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task RegisterAsync_WhenDataIsValid_CreatesUserAndReturnsTokens()
        {
            var password = "correct_password";
            var testUser = UserFaker.Create(email: "jan@test.com", password: password);

            _usersMock.Setup(x => x.GetUserByEmailAsync("jan@test.com"))
                      .ReturnsAsync((User?)null);
            _usersMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
                       .ReturnsAsync(testUser);

            _refreshTokenServiceMock.Setup(x => x.CreateTokenAsync(testUser.Id))
                                    .ReturnsAsync(ResultT<string>.Success("fake-refresh-token"));
            _tokenProviderMock.Setup(x => x.GenerateJwtToken(testUser))
                              .Returns("fake-jwt-token");

            var result = await _sut.RegisterAsync("jan@test.com", password, "Jan", "Kowalski");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(("fake-jwt-token", "fake-refresh-token"));

            _usersMock.Verify(x => x.CreateAsync(It.Is<User>(u =>
                u.Email == "jan@test.com" &&
                u.Firstname == "Jan" &&
                u.Lastname == "Kowalski" &&
                BCrypt.Net.BCrypt.Verify(password, u.PasswordHash) 
            )), Times.Once);
            _refreshTokenServiceMock.Verify(x => x.CreateTokenAsync(testUser.Id), Times.Once);
        }
        #endregion
    }
}
