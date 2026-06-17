using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Providers;
using RoomReservation.Core.Repositories;
using RoomReservation.Core.Services;
using System.Text.Json;

namespace RoomReservation.Api.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration config) 
        {
            services.AddOpenApi();
            services.AddSwagger();
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<ITokenProvider, TokenProvider>();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(config.GetConnectionString("DefaultConnection"));
            });

            services.AddJwtConfiguration(config);
            services.AddControllers(options =>
            {
                options.ValueProviderFactories.Add(new FormValueProviderFactory());
            }).AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            });

            return services;
        }
    }
}
