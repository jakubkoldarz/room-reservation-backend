using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Models.Rooms;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Interfaces
{
    public interface IRoomService
    {
        Task<ResultT<Room>> GetByIdAsync(Guid roomId);
        Task<PagedResult<Room>> GetAllAsync(RoomFilter filters);
        Task<ResultT<Room>> CreateAsync(RoomModel request);
        Task<ResultT<Room>> UpdateAsync(Guid roomId, RoomModel request, bool force = false);
        Task<Result> DeleteAsync(Guid roomId, bool force = false);
    }
}
