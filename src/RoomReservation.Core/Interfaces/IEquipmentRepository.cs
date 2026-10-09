using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Models;

namespace RoomReservation.Core.Interfaces
{
    public interface IEquipmentRepository
    {
        Task<bool> ExistsByNameAsync(string name);
        Task<Equipment?> GetByIdAsync(Guid equipmentId);
        Task<Equipment?> GetByNameAsync(string name);
        Task<PagedList<Equipment>> GetAllAsync(EquipmentFilter filters);
        void Add(Equipment equipment);
        void Remove(Equipment equipment);
        Task<bool> AllExistAsync(IReadOnlyList<Guid> equipmentIds);
    }
}
