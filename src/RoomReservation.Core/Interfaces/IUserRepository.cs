using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Models;

namespace RoomReservation.Core.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid userId);
        void Add(User user);
        Task<PagedList<User>> GetFilteredAsync(UserFilter filters);
    }
}
