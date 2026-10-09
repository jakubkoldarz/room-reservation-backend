using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Interfaces
{
    public interface IJobRepository
    {
        Task<Job?> GetByIdAsync(Guid jobId);
        Task<Job?> TryClaimNextJobAsync();
        void Add(Job job);
    }
}
