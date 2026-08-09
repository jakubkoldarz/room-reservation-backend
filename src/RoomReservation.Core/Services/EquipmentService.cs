using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public class EquipmentService(IEquipmentRepository _equipment) : IEquipmentService
    {
        public async Task<ResultT<Equipment>> CreateAsync(string name, string icon)
        {
            var existingEquipment = await _equipment.ExistsByNameAsync(name);
            if (existingEquipment)
                return new Error("Equipment with the same name already exists.", ErrorType.Conflict);

            var equipmentToAdd = new Equipment
            {
                Name = name,
                Icon = icon
            };

            await _equipment.AddAsync(equipmentToAdd);
            return ResultT<Equipment>.Success(equipmentToAdd);
        }

        public async Task<Result> DeleteAsync(Guid equipmentId)
        {
            var existingEquipment = await _equipment.GetByIdAsync(equipmentId);
            if (existingEquipment is null)
                return new Error("Equipment not found.", ErrorType.NotFound);

            await _equipment.DeleteAsync(existingEquipment);
            return Result.Success();
        }

        public async Task<PagedResult<Equipment>> GetAllAsync(EquipmentFilter filters)
        {
            var equipment = await _equipment.GetAllAsync(filters);
            return PagedResult<Equipment>.Success(equipment.Equipments, equipment.TotalCount, filters.Page, filters.PageSize);
        }

        public async Task<ResultT<Equipment>> GetByIdAsync(Guid equipmentId)
        {
            var existingEquipment = await _equipment.GetByIdAsync(equipmentId);
            if (existingEquipment is null)
                return new Error("Equipment not found.", ErrorType.NotFound);

            return ResultT<Equipment>.Success(existingEquipment);
        }

        public async Task<ResultT<Equipment>> UpdateAsync(Guid equipmentId, string name, string icon)
        {
            var equipmentToUpdate = await _equipment.GetByIdAsync(equipmentId);
            if (equipmentToUpdate is null)
                return new Error("Equipment not found.", ErrorType.NotFound);

            var existingEquipment = await _equipment.GetByNameAsync(name);
            if ((existingEquipment is not null) && existingEquipment.Id != equipmentId)
                return new Error("Equipment with the same name already exists.", ErrorType.Conflict);

            equipmentToUpdate.Name = name;
            equipmentToUpdate.Icon = icon;

            await _equipment.UpdateAsync(equipmentToUpdate);
            return ResultT<Equipment>.Success(equipmentToUpdate);
        }
    }
}
