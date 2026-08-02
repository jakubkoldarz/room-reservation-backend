using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Interfaces
{
    public interface IBuildingService
    {
        Task<ResultT<Building>> GetByIdAsync(Guid buildingId);
        Task<PagedResult<Building>> GetAllAsync(BuildingFilter filters);
        Task<ResultT<Building>> CreateAsync(string name, string? identifier, string street, string city, string postalCode, int floorsCount);
        Task<ResultT<Building>> UpdateAsync(Guid buildingId, string name, string? identifier, string street, string city, string postalCode, int floorsCount);
        Task<Result> DeleteAsync(Guid buildingId);
    }
}
