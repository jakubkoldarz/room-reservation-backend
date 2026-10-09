using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Models.Rooms;
using RoomReservation.Core.Results.Common;


namespace RoomReservation.Core.Services
{
    public class RoomService(
        IRoomRepository roomRepository,
        IAvailabilityService availabilityService,
        IReservationService reservationService,
        IReservationRepository reservationRepository,
        IBuildingRepository buildingRepository,
        IEquipmentRepository equipmentRepository,
        IUnitOfWork unitOfWork) : IRoomService
    {
        public async Task<ResultT<Room>> CreateAsync(RoomModel model)
        {
            var equipmentValidationResult = await AreEquipmentsValid(model.EquipmentIds);
            if (!equipmentValidationResult.IsSuccess)
                return equipmentValidationResult.Error;

            var existingBuilding = await buildingRepository.GetByIdAsync(model.BuildingId);
            if (existingBuilding is null)
                return new Error("Building not found", ErrorType.NotFound);

            var existingRoom = await roomRepository.ExistsByIdentifierAsync(model.BuildingId, model.Identifier);
            if (existingRoom)
                return new Error("Room with the same identifier already exists in the building", ErrorType.Conflict);

            var roomAvailabilities = model.Availabilities.Select(a => a.ToEntity()).ToList();

            var validAvailabilities = await availabilityService.AreAvailabilitiesValid(roomAvailabilities, boundingBuildingId: model.BuildingId);
            if (!validAvailabilities.IsSuccess)
                return validAvailabilities.Error;

            var roomToCreate = new Room
            {
                Identifier = model.Identifier,
                RequiresApproval = model.RequiresApproval,
                BuildingId = model.BuildingId,
                Floor = model.Floor,
                Capacity = model.Capacity,
                Availabilities = roomAvailabilities,
                RoomEquipment = [.. model.EquipmentIds.Select(equipmentId => new RoomEquipment
                {
                    EquipmentId = equipmentId
                })]
            };

            roomRepository.Add(roomToCreate);
            await unitOfWork.SaveChangesAsync();

            var createdRoom = await roomRepository.GetByIdAsync(roomToCreate.Id);
            return ResultT<Room>.Success(createdRoom!);
        }

        public async Task<Result> DeleteAsync(Guid roomId, bool force = false)
        {
            var existingRoom = await roomRepository.GetByIdAsync(roomId);
            if (existingRoom is null)
                return new Error("Room not found", ErrorType.NotFound);

            var activeReservations = await reservationRepository.GetActiveFutureByRoomAsync(roomId);

            if (!force && activeReservations.Any())
                return new Error("Room has active reservations and cannot be deleted", ErrorType.Conflict);

            await reservationService.BulkForceCancelAsync(activeReservations, "Sala została usunięta");
            roomRepository.Remove(existingRoom);
            await unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<ResultT<PagedList<Room>>> GetAllAsync(RoomFilter filters)
        {
            if (filters.StartTime.HasValue != filters.EndTime.HasValue)
                return new Error("StartTime and EndTime must be provided together", ErrorType.BadRequest);

            if (filters.StartTime.HasValue && filters.EndTime.HasValue && filters.StartTime >= filters.EndTime)
                return new Error("StartTime must be earlier than EndTime", ErrorType.BadRequest);

            var rooms = await roomRepository.GetFilteredAsync(filters);
            return ResultT<PagedList<Room>>.Success(rooms);
        }

        public async Task<ResultT<Room>> GetByIdAsync(Guid roomId)
        {
            var room = await roomRepository.GetByIdAsync(roomId);
            if (room is null)
                return new Error("Room not found", ErrorType.NotFound);

            return ResultT<Room>.Success(room);
        }
        public async Task<ResultT<Room>> UpdateAsync(Guid roomId, RoomModel model, bool force = false)
        {
            var toUpdate = await roomRepository.GetByIdAsync(roomId);
            if (toUpdate is null)
                return new Error("Room not found", ErrorType.NotFound);

            var equipmentValidationResult = await AreEquipmentsValid(model.EquipmentIds);
            if (!equipmentValidationResult.IsSuccess)
                return equipmentValidationResult.Error;

            var existingBuilding = await buildingRepository.GetByIdAsync(model.BuildingId);
            if (existingBuilding is null)
                return new Error("Building not found", ErrorType.NotFound);

            var existingRoom = await roomRepository.ExistsByIdentifierAsync(model.BuildingId, model.Identifier, toUpdate.Id);
            if (existingRoom)
                return new Error("Room with the same identifier already exists in the building", ErrorType.Conflict);

            toUpdate.Identifier = model.Identifier;
            toUpdate.RequiresApproval = model.RequiresApproval;
            toUpdate.BuildingId = model.BuildingId;
            toUpdate.Floor = model.Floor;
            toUpdate.Capacity = model.Capacity;
            ReplaceEquipment(toUpdate, model.EquipmentIds);

            var replacementResult = await availabilityService.ReplaceIfValidForRoomAsync(toUpdate, model.Availabilities, force);
            if (!replacementResult.IsSuccess)
                return replacementResult.Error;

            await reservationService.BulkForceCancelAsync(replacementResult.Value, "Zmiany administracyjne w godzinach dostępności sal");
            await unitOfWork.SaveChangesAsync();

            return ResultT<Room>.Success(toUpdate);
        }

        private static void ReplaceEquipment(Room room, IReadOnlyList<Guid> equipmentIds)
        {
            var toRemove = room.RoomEquipment.Where(re => !equipmentIds.Contains(re.EquipmentId)).ToList();
            foreach (var roomEquipment in toRemove)
                room.RoomEquipment.Remove(roomEquipment);

            var toAdd = equipmentIds.Where(id => room.RoomEquipment.All(re => re.EquipmentId != id));
            foreach (var equipmentId in toAdd)
                room.RoomEquipment.Add(new RoomEquipment { EquipmentId = equipmentId });
        }

        private async Task<Result> AreEquipmentsValid(IReadOnlyList<Guid> equipmentIds)
        {
            if (equipmentIds.Distinct().Count() != equipmentIds.Count)
                return new Error("Duplicate equipment IDs are not allowed", ErrorType.BadRequest);

            if (!await equipmentRepository.AllExistAsync(equipmentIds))
                return new Error("One or more equipment IDs are invalid", ErrorType.BadRequest);

            return Result.Success();
        }
    }
}
