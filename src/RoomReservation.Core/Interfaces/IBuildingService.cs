using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Models;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Interfaces
{
    public interface IBuildingService
    {
        Task<ResultT<Building>> GetByIdAsync(Guid buildingId);
        Task<PagedResult<Building>> GetAllAsync(BuildingFilter filters);
        Task<ResultT<Building>> CreateAsync(BuildingModel request);
        Task<ResultT<Building>> UpdateAsync(Guid buildingId, BuildingModel request);
        Task<Result> DeleteAsync(Guid buildingId);
    }
}
