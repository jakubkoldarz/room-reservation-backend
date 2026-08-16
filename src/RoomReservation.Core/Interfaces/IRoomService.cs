using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Models;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Interfaces
{
    public interface IRoomService
    {
        Task<ResultT<Room>> GetByIdAsync(Guid roomId);
        Task<PagedResult<Room>> GetAllAsync(RoomFilter filters);
        Task<ResultT<Room>> CreateAsync(
            string identifier, bool requiresApproval,
            Guid buildingId, int floor, int capacity,
            IReadOnlyList<Guid> equipmentIds, IReadOnlyList<AvailabilitySlot> availabilities);
        Task<ResultT<Room>> UpdateAsync(
           Guid roomId, string identifier, bool requiresApproval,
           Guid buildingId, int floor, int capacity,
           IReadOnlyList<Guid> equipmentIds, IReadOnlyList<AvailabilitySlot> availabilities);
        Task<Result> DeleteAsync(Guid roomId);
        Task<Result> AddSpecialAvailabilityAsync(Guid roomId, SpecialAvailabilitySlot specialAvailability);
        Task<Result> RemoveSpecialAvailabilityAsync(Guid specialAvailabilityId);
    }
}
