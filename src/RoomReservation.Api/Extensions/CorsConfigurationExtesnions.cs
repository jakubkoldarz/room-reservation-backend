namespace RoomReservation.Api.Extensions
{
    public static class CorsConfigurationExtesnions
    {
        public static IServiceCollection AddCorsConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("CORS:ALLOWED_ORIGINS").Get<string[]>();
            if(allowedOrigins is null)
                throw new InvalidOperationException("Allowed origins for CORS are not configured. Please check the configuration.");

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.WithOrigins(allowedOrigins)
                           .WithMethods("GET", "POST", "PUT", "DELETE")
                           .WithHeaders("Authorization", "Content-Type", "Accept")
                           .AllowCredentials();
                });
            });
            return services;
        }
    }
}
