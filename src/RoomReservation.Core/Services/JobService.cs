using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public class JobService(IJobRepository _jobs) : IJobService
    {
        public async Task<Result> EnqueueJobAsync(JobModel model)
        {
            var nextAttemptAt = model.Delay.HasValue ? DateTime.UtcNow.Add(model.Delay.Value) : DateTime.UtcNow;

            var job = new Job
            {
                JobType = model.JobType,
                Payload = model.Payload,
                MaxAttempts = model.MaxAttempts,
                NextAttemptAt = nextAttemptAt
            };

            await _jobs.AddAsync(job);
            return Result.Success();
        }

        public async Task<ResultT<Job>> GetByIdAsync(Guid jobId)
        {
            var job = await _jobs.GetByIdAsync(jobId);
            if(job is null)
                return new Error("Job not found", ErrorType.NotFound);
            return ResultT<Job>.Success(job);
        }

        public async Task<Result> MarkAsCompletedAsync(Job job)
        {
            job.Status = JobStatus.Completed;
            await _jobs.UpdateAsync(job);
            return Result.Success();
        }

        public async Task<Result> MarkAsFailedAsync(Job job, string errorMessage)
        {
            job.Status = JobStatus.Failed;
            job.LastError = errorMessage;
            await _jobs.UpdateAsync(job);
            return Result.Success();
        }

        public async Task<Result> MarkAsPendingAsync(Job job)
        {
            job.Status = JobStatus.Pending;
            await _jobs.UpdateAsync(job);
            return Result.Success();
        }

        public async Task<ResultT<Job>> TryClaimNextJobAsync()
        {
            var job = await _jobs.TryClaimNextJobAsync();
            if(job is null)
                return new Error("No jobs available", ErrorType.NotFound);

            return ResultT<Job>.Success(job);
        }
    }
}
