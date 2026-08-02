using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Interfaces
{
    public interface IUserService
    {
        Task<ResultT<User>> GetUserDetailsAsync(Guid userId);
        Task<PagedResult<User>> GetAllAsync(UserFilter filters);
        Task<ResultT<User>> UpdateUserAsync(Guid userId, string firstname, string lastname);
    }
}
