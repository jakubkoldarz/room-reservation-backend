using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models.Rooms;
using RoomReservation.Core.Results.Common;


namespace RoomReservation.Core.Services
{
    public class RoomService(
        IRoomRepository _rooms,
        IAvailabilityService _availabilityService,
        IReservationService _reservationService,
        IReservationRepository _reservations,
        IBuildingRepository _buildings,
        IEquipmentRepository _equipment) : IRoomService
    {
        public async Task<ResultT<Room>> CreateAsync(RoomModel model)
        {
            var equipmentValidationResult = await AreEquipmentsValid(model.EquipmentIds);
            if (!equipmentValidationResult.IsSuccess)
                return equipmentValidationResult.Error;

            var exisitingBuilding = await _buildings.GetByIdAsync(model.BuildingId);
            if (exisitingBuilding is null)
                return new Error("Building not found", ErrorType.NotFound);

            var existingRoom = await _rooms.ExistsByIdentifierAsync(model.BuildingId, model.Identifier);
            if (existingRoom)
                return new Error("Room with the same identifier already exists in the building", ErrorType.Conflict);

            var roomAvailabilities = model.Availabilities.Select(a => new Availability
            {
                DayOfWeek = a.DayOfWeek,
                StartTime = a.StartTime,
                EndTime = a.EndTime
            }).ToList();

            var validAvailabilities = await _availabilityService.AreAvailabilitiesValid(roomAvailabilities, boundingBuildingId: model.BuildingId);
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

            await _rooms.AddAsync(roomToCreate);
            var createdRoom = await _rooms.GetByIdAsync(roomToCreate.Id);
            return ResultT<Room>.Success(createdRoom!);
        }

        public async Task<Result> DeleteAsync(Guid roomId, bool force = false)
        {
            var existingRoom = await _rooms.GetByIdAsync(roomId);
            if (existingRoom is null)
                return new Error("Room not found", ErrorType.NotFound);

            var activeReservations = await _reservations.GetActiveFutureByRoomAsync(roomId);

            if (!force && activeReservations.Any())
                return new Error("Room has active reservations and cannot be deleted", ErrorType.Conflict);

            await _rooms.DeleteAsync(existingRoom);
            return Result.Success();
        }

        public async Task<PagedResult<Room>> GetAllAsync(RoomFilter filters)
        {
            if ((filters.StartTime.HasValue && !filters.EndTime.HasValue) || (!filters.StartTime.HasValue && filters.EndTime.HasValue))
            {
                return PagedResult<Room>.Failure("StartTime and EndTime must be provided together", ErrorType.BadRequest);
            }

            if (filters.StartTime.HasValue && filters.EndTime.HasValue && filters.StartTime >= filters.EndTime)
            {
                return PagedResult<Room>.Failure("StartTime must be earlier than EndTime", ErrorType.BadRequest);
            }

            var rooms = await _rooms.GetFilteredAsync(filters);
            return PagedResult<Room>.Success(rooms.Rooms, rooms.TotalCount, filters.Page, filters.PageSize);
        }

        public async Task<ResultT<Room>> GetByIdAsync(Guid roomId)
        {
            var room = await _rooms.GetByIdAsync(roomId);
            if (room is null)
                return new Error("Room not found", ErrorType.NotFound);

            return ResultT<Room>.Success(room);
        }
        public async Task<ResultT<Room>> UpdateAsync(Guid roomId, RoomModel model, bool force = false)
        {
            var toUpdate = await _rooms.GetByIdAsync(roomId);
            if (toUpdate is null)
                return new Error("Room not found", ErrorType.NotFound);

            var equipmentValidationResult = await AreEquipmentsValid(model.EquipmentIds);
            if (!equipmentValidationResult.IsSuccess)
                return equipmentValidationResult.Error;

            var existingBuilding = await _buildings.GetByIdAsync(model.BuildingId);
            if (existingBuilding is null)
                return new Error("Building not found", ErrorType.NotFound);

            var existingRoom = await _rooms.ExistsByIdentifierAsync(model.BuildingId, model.Identifier, toUpdate.Id);
            if (existingRoom)
                return new Error("Room with the same identifier already exists in the building", ErrorType.Conflict);

            var replacementResult = await _availabilityService.ReplaceIfValidForRoomAsync(toUpdate, model.Availabilities, force);
            if (!replacementResult.IsSuccess)
                return replacementResult.Error;

            if(replacementResult.Value.Any() && force)
                await _reservationService.BulkForceCancelAsync(replacementResult.Value, "Zmiany administracyjne w godzinach dostępności sal");

            toUpdate.Identifier = model.Identifier;
            toUpdate.RequiresApproval = model.RequiresApproval;
            toUpdate.BuildingId = model.BuildingId;
            toUpdate.Floor = model.Floor;
            toUpdate.Capacity = model.Capacity;
            toUpdate.RoomEquipment = [.. model.EquipmentIds.Select(equipmentId => new RoomEquipment { EquipmentId = equipmentId })];

            await _rooms.UpdateAsync(toUpdate);
            return ResultT<Room>.Success(toUpdate);
        }

        private async Task<Result> AreEquipmentsValid(IReadOnlyList<Guid> equipmentIds)
        {
            if (equipmentIds.Distinct().Count() != equipmentIds.Count)
                return new Error("Duplicate equipment IDs are not allowed", ErrorType.BadRequest);

            if (!await _equipment.AllExistAsync(equipmentIds))
                return new Error("One or more equipment IDs are invalid", ErrorType.BadRequest);

            return Result.Success();
        }
    }
}
