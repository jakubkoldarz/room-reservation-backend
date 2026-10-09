using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Extensions;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RoomReservation.Core.Providers
{
    public class TokenProvider(IOptions<JwtOptions> jwtOptions, TimeProvider timeProvider) : ITokenProvider
    {
        private static readonly TimeSpan JwtLifetime = TimeSpan.FromMinutes(10);

        public string GenerateJwtToken(User user)
        {
            var jwt = jwtOptions.Value;
            var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Secret));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()) }),
                Issuer = jwt.Issuer,
                Audience = jwt.Audience,
                Expires = timeProvider.UtcNow().Add(JwtLifetime),
                SigningCredentials = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static string HashRefreshToken(string token)
        {
            var hash = SHA256.HashData(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hash);
        }

        public (string token, string hash) GenerateRefreshToken()
        {
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var hash = HashRefreshToken(refreshToken);

            return (refreshToken, hash);
        }
    }
}
