using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RoomReservation.Core.Providers
{
    public class TokenProvider(IConfiguration config) : ITokenProvider
    {
        public string GenerateJwtToken(User user)
        {
            var jwtSecret = config["Jwt:Secret"] ?? throw new InvalidOperationException("Missing config: Jwt:Secret");
            var issuer = config["Jwt:Issuer"] ?? throw new InvalidOperationException("Missing config: Jwt:Issuer");
            var audience = config["Jwt:Audience"] ?? throw new InvalidOperationException("Missing config: Jwt:Audience");

            var jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Iss, issuer!),
                new(JwtRegisteredClaimNames.Aud, audience!),
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(10),
                SigningCredentials = new SigningCredentials(jwtKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            throw new NotImplementedException();
        }
    }
}
