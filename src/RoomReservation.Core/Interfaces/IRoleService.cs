using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Models;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Interfaces
{
    public interface IRoleService
    {
        Task<ResultT<Role>> CreateAsync(RoleModel request, bool force = false);
        Task<ResultT<Role>> UpdateAsync(Guid roleId, RoleModel request, bool force = false);
        Task<Result> DeleteAsync(Guid roleId);
        Task<ResultT<Role>> GetByIdAsync(Guid roleId);
        Task<PagedResult<Role>> GetAllAsync(RoleFilter filters);
    }
}
