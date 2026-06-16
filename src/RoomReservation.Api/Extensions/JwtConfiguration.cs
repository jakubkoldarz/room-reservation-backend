using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace RoomReservation.Api.Extensions
{
    public static class JwtConfiguration
    {
        public static IServiceCollection AddJwtConfiguration(this IServiceCollection services, IConfiguration config) 
        {
            var jwtKey = config["Jwt:Secret"]     ?? throw new InvalidOperationException("Missing config: Jwt:Secret");
            var issuer = config["Jwt:Issuer"]     ?? throw new InvalidOperationException("Missing config: Jwt:Issuer");
            var audience = config["Jwt:Audience"] ?? throw new InvalidOperationException("Missing config: Jwt:Audience");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
                    };
                });

            return services;
        }
    }
}
