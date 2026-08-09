using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;

namespace RoomReservation.Core.Interfaces
{
    public interface IEquipmentRepository
    {
        Task<bool> ExistsByNameAsync(string name);
        Task<Equipment?> GetByIdAsync(Guid equipmentId);
        Task<Equipment?> GetByNameAsync(string name);
        Task<(IReadOnlyList<Equipment> Equipments, int TotalCount)> GetAllAsync(EquipmentFilter filters);
        Task AddAsync(Equipment equipment);
        Task UpdateAsync(Equipment equipment);
        Task DeleteAsync(Equipment equipment);
    }
}
