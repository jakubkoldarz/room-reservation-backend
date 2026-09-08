using System.Threading.RateLimiting;

namespace RoomReservation.Api.Extensions
{
    public static class RateLimiterExtensions
    {
        public static IServiceCollection AddRateLimiterPolicies(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                options.AddPolicy("strict", httpContext =>
                {
                    return RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromMinutes(1),
                            PermitLimit = 5,
                            SegmentsPerWindow = 4,
                            QueueLimit = 0
                        }
                    );
                });

                options.AddPolicy("default", httpContext =>
                {
                    return RateLimitPartition.GetSlidingWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        factory: _ => new SlidingWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromMinutes(1),
                            PermitLimit = 100,
                            SegmentsPerWindow = 4,
                            QueueLimit = 0
                        }
                    );
                });
            });

            return services;
        }
    }
}
