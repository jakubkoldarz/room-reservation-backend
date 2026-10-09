using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public class EquipmentService(IEquipmentRepository equipmentRepository, IUnitOfWork unitOfWork) : IEquipmentService
    {
        public async Task<ResultT<Equipment>> CreateAsync(string name, string icon)
        {
            var existingEquipment = await equipmentRepository.ExistsByNameAsync(name);
            if (existingEquipment)
                return new Error("Equipment with the same name already exists.", ErrorType.Conflict);

            var equipmentToAdd = new Equipment
            {
                Name = name,
                Icon = icon
            };

            equipmentRepository.Add(equipmentToAdd);
            await unitOfWork.SaveChangesAsync();
            return ResultT<Equipment>.Success(equipmentToAdd);
        }

        public async Task<Result> DeleteAsync(Guid equipmentId)
        {
            var existingEquipment = await equipmentRepository.GetByIdAsync(equipmentId);
            if (existingEquipment is null)
                return new Error("Equipment not found.", ErrorType.NotFound);

            equipmentRepository.Remove(existingEquipment);
            await unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<ResultT<PagedList<Equipment>>> GetAllAsync(EquipmentFilter filters)
        {
            var equipment = await equipmentRepository.GetAllAsync(filters);
            return ResultT<PagedList<Equipment>>.Success(equipment);
        }

        public async Task<ResultT<Equipment>> GetByIdAsync(Guid equipmentId)
        {
            var existingEquipment = await equipmentRepository.GetByIdAsync(equipmentId);
            if (existingEquipment is null)
                return new Error("Equipment not found.", ErrorType.NotFound);

            return ResultT<Equipment>.Success(existingEquipment);
        }

        public async Task<ResultT<Equipment>> UpdateAsync(Guid equipmentId, string name, string icon)
        {
            var equipmentToUpdate = await equipmentRepository.GetByIdAsync(equipmentId);
            if (equipmentToUpdate is null)
                return new Error("Equipment not found.", ErrorType.NotFound);

            var existingEquipment = await equipmentRepository.GetByNameAsync(name);
            if ((existingEquipment is not null) && existingEquipment.Id != equipmentId)
                return new Error("Equipment with the same name already exists.", ErrorType.Conflict);

            equipmentToUpdate.Name = name;
            equipmentToUpdate.Icon = icon;

            await unitOfWork.SaveChangesAsync();
            return ResultT<Equipment>.Success(equipmentToUpdate);
        }
    }
}
