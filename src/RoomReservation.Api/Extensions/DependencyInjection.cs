using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using RoomReservation.Api.Authorization;
using RoomReservation.Api.Authorization.Handlers;
using RoomReservation.Core.Data;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Providers;
using RoomReservation.Core.Repositories;
using RoomReservation.Core.Services;

namespace RoomReservation.Api.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCore(this IServiceCollection services, IConfiguration config)
        {
            services.AddOpenApi();
            services.AddSwagger();
            services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

            services.AddSingleton(TimeProvider.System);

            services.AddSingleton<IAuthorizationPolicyProvider, CustomPolicyProvider>();
            services.AddScoped<UserAccessProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddScoped<IAuthorizationHandler, ProfileCompletedHandler>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();

            services.AddScoped<IVerificationCodeRepository, VerificationCodeRepository>();
            services.AddScoped<IVerificationCodeService, VerificationCodeService>();

            services.AddScoped<ITokenProvider, TokenProvider>();
            services.AddScoped<IEmailQueue, EmailQueue>();

            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped<IPermissionService, PermissionService>();

            services.AddScoped<IBuildingRepository, BuildingRepository>();
            services.AddScoped<IBuildingService, BuildingService>();

            services.AddScoped<IEquipmentRepository, EquipmentRepository>();
            services.AddScoped<IEquipmentService, EquipmentService>();

            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IRoomService, RoomService>();

            services.AddScoped<IReservationRepository, ReservationRepository>();
            services.AddScoped<IReservationService, ReservationService>();

            services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
            services.AddScoped<IAvailabilityService, AvailabilityService>();

            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IEventService, EventService>();

            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleService, RoleService>();

            services.AddScoped<IJobRepository, JobRepository>();
            services.AddScoped<IJobService, JobService>();

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(config.GetConnectionString("DefaultConnection"));
            });

            services.AddJwtConfiguration();
            services.AddControllers();

            return services;
        }
    }
}
