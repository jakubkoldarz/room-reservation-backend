using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;

namespace RoomReservation.Worker.Jobs
{
    public class JobDispatcher(IEnumerable<IJobHandler> handlers)
    {
        private readonly Dictionary<JobTypes, IJobHandler> _handlers = handlers.ToDictionary(h => h.JobType);

        public async Task DispatchAsync(Job job, CancellationToken ct)
        {
            if (!_handlers.TryGetValue(job.JobType, out var handler))
                throw new InvalidOperationException($"Brak handlera dla typu joba: {job.JobType}");

            await handler.HandleAsync(job.Payload, ct);
        }
    }
}
