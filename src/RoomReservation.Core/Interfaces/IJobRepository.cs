using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Interfaces
{
    public interface IJobRepository
    {
        Task<Job?> GetByIdAsync(Guid jobId);
        Task<Job?> TryClaimNextJobAsync();
        Task UpdateAsync(Job job);
        Task AddAsync(Job job);
    }
}
