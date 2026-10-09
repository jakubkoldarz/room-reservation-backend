using RoomReservation.Core.Interfaces;
using RoomReservation.Worker.Jobs;

namespace RoomReservation.Worker
{
    public class Worker(IServiceScopeFactory scopeFactory, ILogger<Worker> logger) : BackgroundService
    {
        private static readonly TimeSpan IdleDelay = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan ErrorDelay = TimeSpan.FromSeconds(5);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var processed = await ProcessNextJobAsync(stoppingToken);
                    if (!processed)
                        await Task.Delay(IdleDelay, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Worker iteration failed");
                    await Task.Delay(ErrorDelay, stoppingToken);
                }
            }
        }

        private async Task<bool> ProcessNextJobAsync(CancellationToken stoppingToken)
        {
            using var scope = scopeFactory.CreateScope();
            var jobService = scope.ServiceProvider.GetRequiredService<IJobService>();
            var dispatcher = scope.ServiceProvider.GetRequiredService<JobDispatcher>();

            var claimResult = await jobService.TryClaimNextJobAsync();
            if (!claimResult.IsSuccess)
                return false;

            var job = claimResult.Value;

            try
            {
                await dispatcher.DispatchAsync(job, stoppingToken);
                await jobService.MarkAsCompletedAsync(job);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                logger.LogError(ex, "Job {JobId} of type {JobType} failed", job.Id, job.JobType);
                await jobService.MarkAsFailedAsync(job, ex.Message);
            }

            return true;
        }
    }
}
