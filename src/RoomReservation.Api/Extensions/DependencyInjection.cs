using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using RoomReservation.Api.Authorization;
using RoomReservation.Api.Authorization.Handlers;
using RoomReservation.Api.Dtos;
using RoomReservation.Core.Authorization.Handlers;
using RoomReservation.Core.Data;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Providers;
using RoomReservation.Core.Repositories;
using RoomReservation.Core.Services;
using System.Net;
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

            services.AddSingleton<IAuthorizationPolicyProvider, CustomPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddScoped<IAuthorizationHandler, ProfileCompletedHandler>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();

            services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
            services.AddScoped<IVerificationCodeService, VerificationCodeService>();

            services.AddScoped<ITokenProvider, TokenProvider>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IPermissionService, PermissionService>();

            services.AddScoped<IBuildingRepository, BuildingRepository>();
            services.AddScoped<IBuildingService, BuildingService>();

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
            }).ConfigureApiBehaviorOptions(options =>
            {
                options.InvalidModelStateResponseFactory = (context) =>
                {
                    var error = context.ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .SelectMany(x => x.Value!.Errors)
                        .Select(x => x.ErrorMessage)
                        .FirstOrDefault() ?? "Validation error";

                    var response = new ErrorResponse(
                        error,
                        HttpStatusCode.BadRequest
                    );

                    return new BadRequestObjectResult(response);
                };
            });

            return services;
        }
    }
}
