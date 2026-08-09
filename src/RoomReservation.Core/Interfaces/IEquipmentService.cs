using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Interfaces
{
    public interface IEquipmentService
    {
        Task<ResultT<Equipment>> GetByIdAsync(Guid equipmentId);
        Task<PagedResult<Equipment>> GetAllAsync(EquipmentFilter filters);
        Task<ResultT<Equipment>> CreateAsync(string name, string icon);
        Task<ResultT<Equipment>> UpdateAsync(Guid equipmentId, string name, string icon);
        Task<Result> DeleteAsync(Guid equipmentId);
    }
}
