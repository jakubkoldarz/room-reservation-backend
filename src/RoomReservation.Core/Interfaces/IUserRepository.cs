using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;

namespace RoomReservation.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid userId);
        void Add(User user);
        Task<bool> IsProfileCompletedAsync(Guid userId);
        Task<(IReadOnlyList<User> Users, int TotalCount)> GetFilteredAsync(UserFilter filters);
    }
}
