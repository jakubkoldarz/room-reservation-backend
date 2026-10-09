using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Extensions;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public class JobService(IJobRepository jobRepository, IUnitOfWork unitOfWork, TimeProvider timeProvider) : IJobService
    {
        private static readonly TimeSpan BaseRetryDelay = TimeSpan.FromSeconds(30);

        public void Enqueue(JobModel model)
        {
            var now = timeProvider.UtcNow();
            var nextAttemptAt = model.Delay.HasValue ? now.Add(model.Delay.Value) : now;

            var job = new Job
            {
                JobType = model.JobType,
                Payload = model.Payload,
                MaxAttempts = model.MaxAttempts,
                CreatedAt = now,
                NextAttemptAt = nextAttemptAt
            };

            jobRepository.Add(job);
        }

        public async Task<ResultT<Job>> GetByIdAsync(Guid jobId)
        {
            var job = await jobRepository.GetByIdAsync(jobId);
            if(job is null)
                return new Error("Job not found", ErrorType.NotFound);
            return ResultT<Job>.Success(job);
        }

        public async Task<Result> MarkAsCompletedAsync(Job job)
        {
            job.Status = JobStatus.Completed;
            job.ProcessedAt = timeProvider.UtcNow();
            await unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> MarkAsFailedAsync(Job job, string errorMessage)
        {
            var retryDelay = BaseRetryDelay * Math.Pow(2, Math.Max(job.Attempts - 1, 0));

            job.Status = JobStatus.Failed;
            job.LastError = errorMessage;
            job.NextAttemptAt = timeProvider.UtcNow().Add(retryDelay);
            await unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> MarkAsPendingAsync(Job job)
        {
            job.Status = JobStatus.Pending;
            job.NextAttemptAt = timeProvider.UtcNow();
            await unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<ResultT<Job>> TryClaimNextJobAsync()
        {
            var job = await jobRepository.TryClaimNextJobAsync();
            if(job is null)
                return new Error("No jobs available", ErrorType.NotFound);

            return ResultT<Job>.Success(job);
        }
    }
}
