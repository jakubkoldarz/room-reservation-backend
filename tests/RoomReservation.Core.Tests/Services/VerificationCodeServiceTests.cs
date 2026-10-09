using FluentAssertions;
using Moq;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Services;
using RoomReservation.Core.Tests.TestHelpers;

namespace RoomReservation.Core.Tests.Services
{
    public class VerificationCodeServiceTests
    {
        private readonly Mock<IVerificationCodeRepository> _verificationCodesMock = new();
        private readonly FixedTimeProvider _timeProvider = new();
        private readonly VerificationCodeService _sut;

        private static readonly DateTime Now = FixedTimeProvider.DefaultNow.UtcDateTime;

        public VerificationCodeServiceTests()
        {
            _sut = new(_verificationCodesMock.Object, _timeProvider);
        }

        private VerificationCode SetupCode(VerificationCodeType type = VerificationCodeType.TwoFactorLogin, DateTime? expiresAt = null)
        {
            var code = new VerificationCode
            {
                UserId = Guid.NewGuid(),
                Code = "123456",
                Type = type,
                ExpiresAt = expiresAt ?? Now.AddMinutes(5)
            };
            _verificationCodesMock.Setup(x => x.GetByIdAsync(code.Id)).ReturnsAsync(code);
            return code;
        }

        #region GenerateCodeAsync

        [Theory]
        [InlineData(VerificationCodeType.EmailActivation, 15)]
        [InlineData(VerificationCodeType.TwoFactorLogin, 5)]
        [InlineData(VerificationCodeType.ChangeEmail, 10)]
        public async Task GenerateCodeAsync_CreatesSixDigitCodeWithTypeSpecificExpiry(VerificationCodeType type, int expectedMinutes)
        {
            var userId = Guid.NewGuid();

            var result = await _sut.GenerateCodeAsync(userId, type);

            result.IsSuccess.Should().BeTrue();
            result.Value!.Code.Should().MatchRegex("^[0-9]{6}$");
            result.Value.ExpiresAt.Should().Be(Now.AddMinutes(expectedMinutes));
            _verificationCodesMock.Verify(x => x.InvalidateActiveCodesAsync(userId, type), Times.Once);
            _verificationCodesMock.Verify(x => x.Add(result.Value), Times.Once);
        }

        #endregion
        #region ValidateCodeAsync

        [Fact]
        public async Task ValidateCodeAsync_WhenCodeIsUsedTwice_SecondAttemptFails()
        {
            var code = SetupCode();

            var first = await _sut.ValidateCodeAsync(code.Id, "123456", VerificationCodeType.TwoFactorLogin);
            var second = await _sut.ValidateCodeAsync(code.Id, "123456", VerificationCodeType.TwoFactorLogin);

            first.IsSuccess.Should().BeTrue();
            code.IsUsed.Should().BeTrue();
            second.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateCodeAsync_WhenVerificationIdDoesNotExist_ReturnsNotFound()
        {
            _verificationCodesMock.Setup(x => x.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((VerificationCode?)null);

            var result = await _sut.ValidateCodeAsync(Guid.NewGuid(), "123456", VerificationCodeType.TwoFactorLogin);

            result.Error!.ErrorType.Should().Be(ErrorType.NotFound);
        }

        [Fact]
        public async Task ValidateCodeAsync_WhenCodeIsWrong_FailsAndDoesNotMarkAsUsed()
        {
            var code = SetupCode();

            var result = await _sut.ValidateCodeAsync(code.Id, "000000", VerificationCodeType.TwoFactorLogin);

            result.Error!.ErrorType.Should().Be(ErrorType.BadRequest);
            code.IsUsed.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateCodeAsync_WhenCodeExpired_Fails()
        {
            var code = SetupCode(expiresAt: Now);

            var result = await _sut.ValidateCodeAsync(code.Id, "123456", VerificationCodeType.TwoFactorLogin);

            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public async Task ValidateCodeAsync_WhenTypeDoesNotMatch_Fails()
        {
            var code = SetupCode(type: VerificationCodeType.ChangeEmail);

            var result = await _sut.ValidateCodeAsync(code.Id, "123456", VerificationCodeType.TwoFactorLogin);

            result.IsSuccess.Should().BeFalse();
        }

        #endregion
    }
}
