using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Providers;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public class RoomService(IRoomRepository _rooms, IBuildingRepository _buildings, IEquipmentRepository _equipment) : IRoomService
    {
        public async Task<ResultT<Room>> CreateAsync(string identifier, bool requiresApproval, Guid buildingId, int floor, int capacity, IReadOnlyList<Guid> equipmentIds, IReadOnlyList<AvailabilitySlot> availabilities)
        {
            if (equipmentIds.Distinct().Count() != equipmentIds.Count)
                return new Error("Duplicate equipment IDs are not allowed", ErrorType.BadRequest);

            if (!await _equipment.AllExistAsync(equipmentIds))
                return new Error("One or more equipment IDs are invalid", ErrorType.BadRequest);

            var exisitingBuilding = await _buildings.GetByIdAsync(buildingId);
            if (exisitingBuilding is null)
                return new Error("Building not found", ErrorType.NotFound);

            var existingRoom = await _rooms.ExistsByIdentifierAsync(buildingId, identifier);
            if (existingRoom)
                return new Error("Room with the same identifier already exists in the building", ErrorType.Conflict);

            var validAvailabilities = AvailabilityProvider.EnsureValidAvailabilities(availabilities);
            if (!validAvailabilities)
                return new Error("Invalid availabilities provided", ErrorType.BadRequest);

            var roomToCreate = new Room
            {
                Identifier = identifier,
                RequiresApproval = requiresApproval,
                BuildingId = buildingId,
                Floor = floor,
                Capacity = capacity,
                Availabilities = [.. availabilities.Select(a => new RoomAvailability
                {
                    DayOfWeek = a.DayOfWeek,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime
                })],
                RoomEquipment = [.. equipmentIds.Select(equipmentId => new RoomEquipment
                {
                    EquipmentId = equipmentId
                })]
            };

            await _rooms.AddAsync(roomToCreate);
            var createdRoom = await _rooms.GetByIdAsync(roomToCreate.Id);
            return ResultT<Room>.Success(createdRoom!);
        }

        public async Task<Result> DeleteAsync(Guid roomId)
        {
            var existingRoom = await _rooms.GetByIdAsync(roomId);
            if (existingRoom is null)
                return new Error("Room not found", ErrorType.NotFound);

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

        public async Task<ResultT<Room>> UpdateAsync(Guid roomId, string identifier, bool requiresApproval, Guid buildingId, int floor, int capacity, IReadOnlyList<Guid> equipmentIds, IReadOnlyList<AvailabilitySlot> availabilities)
        {
            if (equipmentIds.Distinct().Count() != equipmentIds.Count)
                return new Error("Duplicate equipment IDs are not allowed", ErrorType.BadRequest);

            if (!await _equipment.AllExistAsync(equipmentIds))
                return new Error("One or more equipment IDs are invalid", ErrorType.BadRequest);

            var roomToUpdate = await _rooms.GetByIdAsync(roomId);
            if (roomToUpdate is null)
                return new Error("Room not found", ErrorType.NotFound);

            var existingRoom = await _rooms.GetByIdentifierAsync(buildingId, identifier);
            if ((existingRoom is not null) && (existingRoom.Id != roomId))
                return new Error("Room with the same identifier already exists in the building", ErrorType.Conflict);

            var validAvailabilities = AvailabilityProvider.EnsureValidAvailabilities(availabilities);
            if (!validAvailabilities)
                return new Error("Invalid availabilities provided", ErrorType.BadRequest);

            roomToUpdate.Identifier = identifier;
            roomToUpdate.RequiresApproval = requiresApproval;
            roomToUpdate.BuildingId = buildingId;
            roomToUpdate.Floor = floor;
            roomToUpdate.Capacity = capacity;

            roomToUpdate.RoomEquipment = [.. equipmentIds.Select(equipmentId => new RoomEquipment
            {
                EquipmentId = equipmentId
            })];
            roomToUpdate.Availabilities = [.. availabilities.Select(a => new RoomAvailability
            {
                DayOfWeek = a.DayOfWeek,
                StartTime = a.StartTime,
                EndTime = a.EndTime
            })];

            await _rooms.UpdateAsync(roomToUpdate);
            var updatedRoom = await _rooms.GetByIdAsync(roomId);
            return ResultT<Room>.Success(updatedRoom!);
        }
    }
}
