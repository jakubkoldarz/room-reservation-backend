using RoomReservation.Core.Enums;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Worker.Jobs
{
    public interface IJobHandler
    {
        JobTypes JobType { get; }
        Task<Result> HandleAsync(string payload, CancellationToken ct);
    }
}
