using RoomReservation.Core.Interfaces;
using RoomReservation.Worker.Jobs;

namespace RoomReservation.Worker
{
    public class Worker(IServiceScopeFactory _scopeFactory, ILogger<Worker> _logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();
                var jobService = scope.ServiceProvider.GetRequiredService<IJobService>();
                var dispatcher = scope.ServiceProvider.GetRequiredService<JobDispatcher>();

                var claimResult = await jobService.TryClaimNextJobAsync();
                if (!claimResult.IsSuccess)
                {
                    await Task.Delay(1000, stoppingToken);
                    continue;
                }

                var job = claimResult.Value;

                try
                {
                    await dispatcher.DispatchAsync(job, stoppingToken);
                    await jobService.MarkAsCompletedAsync(job);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Job {JobId} of type {JobType} failed", job.Id, job.JobType);
                    await jobService.MarkAsFailedAsync(job, ex.Message);
                }
            }
        }
    }
}
