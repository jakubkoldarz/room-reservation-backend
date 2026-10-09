using FluentAssertions;
using Moq;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Services;

namespace RoomReservation.Core.Tests.Services
{
    public class VerificationCodeServiceTests
    {
        private readonly Mock<IVerificationCodeRepository> _verificationCodesMock = new();
        private readonly VerificationCodeService _sut;

        public VerificationCodeServiceTests()
        {
            _sut = new(_verificationCodesMock.Object);
        }

        [Fact]
        public async Task ValidateCodeAsync_WhenCodeIsUsedTwice_SecondAttemptFails()
        {
            var code = new VerificationCode
            {
                UserId = Guid.NewGuid(),
                Code = "123456",
                Type = VerificationCodeType.TwoFactorLogin,
                ExpiresAt = DateTime.UtcNow.AddMinutes(5)
            };
            _verificationCodesMock.Setup(x => x.GetByIdAsync(code.Id))
                                  .ReturnsAsync(code);

            var first = await _sut.ValidateCodeAsync(code.Id, "123456", VerificationCodeType.TwoFactorLogin);
            var second = await _sut.ValidateCodeAsync(code.Id, "123456", VerificationCodeType.TwoFactorLogin);

            first.IsSuccess.Should().BeTrue();
            code.IsUsed.Should().BeTrue();
            second.IsSuccess.Should().BeFalse();
        }
    }
}
