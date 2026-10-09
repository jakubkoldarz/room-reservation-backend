using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using RoomReservation.Core.Options;
using RoomReservation.Core.Providers;
using RoomReservation.Core.Tests.TestHelpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RoomReservation.Core.Tests.Providers
{
    public class TokenProviderTests
    {
        private static readonly JwtOptions JwtOptions = new()
        {
            Secret = "test-secret-that-is-at-least-32-characters-long",
            Issuer = "room-reservation-tests",
            Audience = "room-reservation-clients"
        };

        private readonly TokenProvider _sut = new(Microsoft.Extensions.Options.Options.Create(JwtOptions), TimeProvider.System);

        [Fact]
        public void GenerateJwtToken_CreatesTokenThatPassesValidationAndContainsUserId()
        {
            var user = UserFaker.Create();

            var token = _sut.GenerateJwtToken(user);

            var principal = new JwtSecurityTokenHandler().ValidateToken(token, new TokenValidationParameters
            {
                ValidIssuer = JwtOptions.Issuer,
                ValidAudience = JwtOptions.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtOptions.Secret))
            }, out _);

            principal.FindFirst(ClaimTypes.NameIdentifier)!.Value.Should().Be(user.Id.ToString());
        }

        [Fact]
        public void GenerateJwtToken_ExpiresAfterTenMinutes()
        {
            var token = _sut.GenerateJwtToken(UserFaker.Create());

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);

            jwt.ValidTo.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(10), TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void GenerateRefreshToken_ReturnsUniqueTokenAndMatchingHash()
        {
            var (first, firstHash) = _sut.GenerateRefreshToken();
            var (second, _) = _sut.GenerateRefreshToken();

            first.Should().NotBe(second);
            firstHash.Should().Be(TokenProvider.HashRefreshToken(first));
            firstHash.Should().NotBe(first);
        }
    }
}
