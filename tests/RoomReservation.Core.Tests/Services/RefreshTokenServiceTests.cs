using FluentAssertions;
using Moq;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Providers;
using RoomReservation.Core.Services;
using RoomReservation.Core.Tests.TestHelpers;

namespace RoomReservation.Core.Tests.Services
{
    public class RefreshTokenServiceTests
    {
        private readonly Mock<ITokenProvider> _tokenProviderMock = new();
        private readonly Mock<IRefreshTokenRepository> _refreshTokensMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly FixedTimeProvider _timeProvider = new();

        private readonly RefreshTokenService _sut;

        private static readonly DateTime Now = FixedTimeProvider.DefaultNow.UtcDateTime;

        public RefreshTokenServiceTests()
        {
            _sut = new(_tokenProviderMock.Object, _refreshTokensMock.Object, _unitOfWorkMock.Object, _timeProvider);

            _tokenProviderMock.Setup(x => x.GenerateRefreshToken()).Returns(("new-token", "new-hash"));
            _tokenProviderMock.Setup(x => x.GenerateJwtToken(It.IsAny<User>())).Returns("jwt");
        }

        private static RefreshToken CreateToken(Guid userId, string value, DateTime? expiresAt = null, DateTime? revokedAt = null) => new()
        {
            UserId = userId,
            User = UserFaker.Create(id: userId),
            TokenHash = TokenProvider.HashRefreshToken(value),
            CreatedAt = Now.AddDays(-1),
            ExpiresAt = expiresAt ?? Now.AddDays(6),
            RevokedAt = revokedAt
        };

        #region CreateToken

        [Fact]
        public void CreateToken_AddsHashedTokenValidForSevenDays()
        {
            var userId = Guid.NewGuid();

            var token = _sut.CreateToken(userId, "127.0.0.1", "agent");

            token.Should().Be("new-token");
            _refreshTokensMock.Verify(x => x.Add(It.Is<RefreshToken>(rt =>
                rt.UserId == userId &&
                rt.TokenHash == "new-hash" &&
                rt.ExpiresAt == Now.AddDays(7) &&
                rt.IpAddress == "127.0.0.1")), Times.Once);
        }

        #endregion
        #region RevokeAsync

        [Fact]
        public async Task RevokeAsync_WhenTokenBelongsToOtherUser_ReturnsNotFound()
        {
            var token = CreateToken(Guid.NewGuid(), "value");
            _refreshTokensMock.Setup(x => x.GetByHashAsync(token.TokenHash)).ReturnsAsync(token);

            var result = await _sut.RevokeAsync(Guid.NewGuid(), "value");

            result.Error!.ErrorType.Should().Be(ErrorType.NotFound);
            token.RevokedAt.Should().BeNull();
        }

        [Fact]
        public async Task RevokeAsync_WhenTokenBelongsToUser_RevokesAndSaves()
        {
            var userId = Guid.NewGuid();
            var token = CreateToken(userId, "value");
            _refreshTokensMock.Setup(x => x.GetByHashAsync(token.TokenHash)).ReturnsAsync(token);

            var result = await _sut.RevokeAsync(userId, "value");

            result.IsSuccess.Should().BeTrue();
            token.RevokedAt.Should().Be(Now);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RevokeAsync_ById_WhenTokenDoesNotExist_ReturnsNotFound()
        {
            _refreshTokensMock.Setup(x => x.GetById(It.IsAny<Guid>())).ReturnsAsync((RefreshToken?)null);

            var result = await _sut.RevokeAsync(Guid.NewGuid(), Guid.NewGuid());

            result.Error!.ErrorType.Should().Be(ErrorType.NotFound);
        }

        #endregion
        #region RotateTokenAsync

        [Fact]
        public async Task RotateTokenAsync_WhenTokenDoesNotExist_ReturnsUnauthorized()
        {
            _refreshTokensMock.Setup(x => x.GetByHashAsync(It.IsAny<string>())).ReturnsAsync((RefreshToken?)null);

            var result = await _sut.RotateTokenAsync("missing");

            result.Error!.ErrorType.Should().Be(ErrorType.Unauthorized);
        }

        [Fact]
        public async Task RotateTokenAsync_WhenTokenWasAlreadyUsed_RevokesAllUserSessions()
        {
            var token = CreateToken(Guid.NewGuid(), "value", revokedAt: Now.AddMinutes(-5));
            _refreshTokensMock.Setup(x => x.GetByHashAsync(token.TokenHash)).ReturnsAsync(token);

            var result = await _sut.RotateTokenAsync("value");

            result.Error!.ErrorType.Should().Be(ErrorType.Unauthorized);
            _refreshTokensMock.Verify(x => x.RevokeAllForUserAsync(token.UserId), Times.Once);
            _refreshTokensMock.Verify(x => x.Add(It.IsAny<RefreshToken>()), Times.Never);
        }

        [Fact]
        public async Task RotateTokenAsync_WhenTokenExpired_ReturnsUnauthorized()
        {
            var token = CreateToken(Guid.NewGuid(), "value", expiresAt: Now.AddSeconds(-1));
            _refreshTokensMock.Setup(x => x.GetByHashAsync(token.TokenHash)).ReturnsAsync(token);

            var result = await _sut.RotateTokenAsync("value");

            result.Error!.ErrorType.Should().Be(ErrorType.Unauthorized);
            _refreshTokensMock.Verify(x => x.Add(It.IsAny<RefreshToken>()), Times.Never);
        }

        [Fact]
        public async Task RotateTokenAsync_WhenTokenIsValid_ReplacesItWithNewOne()
        {
            var token = CreateToken(Guid.NewGuid(), "value");
            RefreshToken? newToken = null;
            _refreshTokensMock.Setup(x => x.GetByHashAsync(token.TokenHash)).ReturnsAsync(token);
            _refreshTokensMock.Setup(x => x.Add(It.IsAny<RefreshToken>())).Callback<RefreshToken>(rt => newToken = rt);

            var result = await _sut.RotateTokenAsync("value");

            result.IsSuccess.Should().BeTrue();
            result.Value.Should().Be(("jwt", "new-token"));
            token.RevokedAt.Should().Be(Now);
            token.ReplacedByTokenId.Should().Be(newToken!.Id);
            newToken.UserId.Should().Be(token.UserId);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        #endregion
    }
}
