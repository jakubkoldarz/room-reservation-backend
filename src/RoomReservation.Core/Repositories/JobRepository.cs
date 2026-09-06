using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Core.Repositories
{
    public class JobRepository(AppDbContext _db) : IJobRepository
    {
        public async Task AddAsync(Job job)
        {
            _db.Add(job);
            await _db.SaveChangesAsync();
        }

        public async Task<Job?> GetByIdAsync(Guid jobId)
        {
            return await _db.Jobs.FindAsync(jobId);
        }

        public async Task<Job?> TryClaimNextJobAsync()
        {
            var jobs = await _db.Jobs
                        .FromSqlInterpolated($@"
                    UPDATE jobs_queue
                    SET ""Status"" = {JobStatus.Processing.ToString()},
                        ""Attempts"" = ""Attempts"" + 1
                    WHERE ""Id"" = (
                        SELECT ""Id"" FROM jobs_queue
                        WHERE ""Status"" = {JobStatus.Pending.ToString()}
                          AND (""NextAttemptAt"" IS NULL OR ""NextAttemptAt"" <= now())
                        ORDER BY ""CreatedAt""
                        LIMIT 1
                        FOR UPDATE SKIP LOCKED
                    )
                    RETURNING *
                ")
                .ToListAsync();

            return jobs.SingleOrDefault();
        }

        public async Task UpdateAsync(Job job)
        {
            _db.Update(job);
            await _db.SaveChangesAsync();
        }
    }
}
