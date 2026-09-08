using RoomReservation.Core.Entities;
using RoomReservation.Core.Models;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Interfaces
{
    public interface IJobService
    {
        Task<Result> EnqueueJobAsync(JobModel model);
        Task<ResultT<Job>> GetByIdAsync(Guid jobId);
        Task<ResultT<Job>> TryClaimNextJobAsync();
        Task<Result> MarkAsFailedAsync(Job job, string errorMessage);
        Task<Result> MarkAsCompletedAsync(Job job);
        Task<Result> MarkAsPendingAsync(Job job);
    }
}
