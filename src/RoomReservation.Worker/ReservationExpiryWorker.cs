using RoomReservation.Core.Interfaces;

namespace RoomReservation.Worker
{
    public class ReservationExpiryWorker(IServiceScopeFactory scopeFactory, ILogger<ReservationExpiryWorker> logger) : BackgroundService
    {
        private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(Interval);

            await ExpireStartedRequestsAsync();
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await ExpireStartedRequestsAsync();
            }
        }

        private async Task ExpireStartedRequestsAsync()
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var expiryService = scope.ServiceProvider.GetRequiredService<IReservationExpiryService>();

                var expiredCount = await expiryService.ExpireStartedRequestsAsync();
                if (expiredCount > 0)
                    logger.LogInformation("Marked {Count} reservation requests as expired", expiredCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Expiring reservation requests failed");
            }
        }
    }
}
