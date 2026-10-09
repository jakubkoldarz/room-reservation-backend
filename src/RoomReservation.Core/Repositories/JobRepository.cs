using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Core.Repositories
{
    public class JobRepository(AppDbContext db) : IJobRepository
    {
        private static readonly TimeSpan LeaseDuration = TimeSpan.FromMinutes(5);

        public void Add(Job job)
            => db.Jobs.Add(job);

        public async Task<Job?> GetByIdAsync(Guid jobId)
        {
            return await db.Jobs.FindAsync(jobId);
        }

        public async Task<Job?> TryClaimNextJobAsync()
        {
            var jobs = await db.Jobs
                        .FromSqlInterpolated($@"
                    UPDATE jobs_queue
                    SET ""Status"" = {JobStatus.Processing.ToString()},
                        ""Attempts"" = ""Attempts"" + 1,
                        ""NextAttemptAt"" = now() + {LeaseDuration}
                    WHERE ""Id"" = (
                        SELECT ""Id"" FROM jobs_queue
                        WHERE (""Status"" = {JobStatus.Pending.ToString()}
                               OR (""Status"" IN ({JobStatus.Failed.ToString()}, {JobStatus.Processing.ToString()}) AND ""Attempts"" < ""MaxAttempts""))
                          AND (""NextAttemptAt"" <= now())
                        ORDER BY ""CreatedAt""
                        LIMIT 1
                        FOR UPDATE SKIP LOCKED
                    )
                    RETURNING *
                ")
                .ToListAsync();

            return jobs.SingleOrDefault();
        }
    }
}
