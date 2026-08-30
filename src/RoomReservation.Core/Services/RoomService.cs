using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public class RoomService(
        IRoomRepository _rooms,
        IAvailabilityService _availabilityService,
        IReservationRepository _reservations,
        IBuildingRepository _buildings,
        IEquipmentRepository _equipment) : IRoomService
    {
        public async Task<ResultT<Room>> CreateAsync(RoomRequest request)
        {
            var equipmentValidationResult = await AreEquipmentsValid(request.EquipmentIds);
            if (!equipmentValidationResult.IsSuccess)
                return equipmentValidationResult.Error;

            var exisitingBuilding = await _buildings.GetByIdAsync(request.BuildingId);
            if (exisitingBuilding is null)
                return new Error("Building not found", ErrorType.NotFound);

            var existingRoom = await _rooms.ExistsByIdentifierAsync(request.BuildingId, request.Identifier);
            if (existingRoom)
                return new Error("Room with the same identifier already exists in the building", ErrorType.Conflict);

            var roomAvailabilities = request.Availabilities.Select(a => new Availability
            {
                DayOfWeek = a.DayOfWeek,
                StartTime = a.StartTime,
                EndTime = a.EndTime
            }).ToList();

            var validAvailabilities = _availabilityService.AreAvailabilitiesValid(roomAvailabilities);
            if (!validAvailabilities)
                return new Error("Invalid availabilities provided", ErrorType.BadRequest);

            var roomToCreate = new Room
            {
                Identifier = request.Identifier,
                RequiresApproval = request.RequiresApproval,
                BuildingId = request.BuildingId,
                Floor = request.Floor,
                Capacity = request.Capacity,
                Availabilities = roomAvailabilities,
                RoomEquipment = [.. request.EquipmentIds.Select(equipmentId => new RoomEquipment
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
        public async Task<ResultT<Room>> UpdateAsync(Guid roomId, RoomRequest request, bool force = false)
        {
            var toUpdate = await _rooms.GetByIdAsync(roomId);
            if (toUpdate is null)
                return new Error("Room not found", ErrorType.NotFound);

            var equipmentValidationResult = await AreEquipmentsValid(request.EquipmentIds);
            if (!equipmentValidationResult.IsSuccess)
                return equipmentValidationResult.Error;

            var existingBuilding = await _buildings.GetByIdAsync(request.BuildingId);
            if (existingBuilding is null)
                return new Error("Building not found", ErrorType.NotFound);

            var existingRoom = await _rooms.ExistsByIdentifierAsync(request.BuildingId, request.Identifier);
            if (existingRoom)
                return new Error("Room with the same identifier already exists in the building", ErrorType.Conflict);

            toUpdate.Identifier = request.Identifier;
            toUpdate.RequiresApproval = request.RequiresApproval;
            toUpdate.BuildingId = request.BuildingId;
            toUpdate.Floor = request.Floor;
            toUpdate.Capacity = request.Capacity;
            toUpdate.RoomEquipment = [.. request.EquipmentIds.Select(equipmentId => new RoomEquipment { EquipmentId = equipmentId })];

            await _rooms.UpdateAsync(toUpdate);

            var availabilityResult = await _availabilityService.ReplaceForRoomAsync(roomId, request.Availabilities, force);
            if (!availabilityResult.IsSuccess)
                return availabilityResult.Error;

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
