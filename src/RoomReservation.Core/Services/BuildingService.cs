using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Models.Availability;
using RoomReservation.Core.Providers;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public class BuildingService(
        IBuildingRepository _buildings,
        IRoomRepository _rooms,
        IAvailabilityService _availabilities) : IBuildingService
    {
        public async Task<ResultT<Building>> CreateAsync(BuildingRequest request)
        {
            var existingBuilding = await _buildings.ExistsByNameAsync(request.Name);
            if (existingBuilding)
                return new Error("Building with the same name already exists", ErrorType.Conflict);

            var availabilityEntities = request.Availabilities.Select(a => new Availability
            {
                DayOfWeek = a.DayOfWeek,
                StartTime = a.StartTime,
                EndTime = a.EndTime
            }).ToList();

            if (!_availabilities.AreAvailabilitiesValid(availabilityEntities))
                return new Error("Invalid availabilities provided", ErrorType.BadRequest);

            var buildingToCreate = new Building
            {
                Name = request.Name,
                Identifier = request.Identifier,
                Street = request.Street,
                City = request.City,
                PostalCode = request.PostalCode,
                FloorsCount = request.FloorsCount,
                Availabilities = availabilityEntities
            };
            await _buildings.AddAsync(buildingToCreate);
            return ResultT<Building>.Success(buildingToCreate);
        }
        public async Task<Result> DeleteAsync(Guid buildingId)
        {
            var existingBuilding = await _buildings.GetByIdAsync(buildingId);
            if (existingBuilding is null)
                return new Error("Building not found", ErrorType.NotFound);

            var hasRooms = await _rooms.GetFilteredAsync(new RoomFilter { BuildingId = buildingId, Page = 1, PageSize = 1 });
            if (hasRooms.TotalCount > 0)
                return new Error("Cannot delete building with associated rooms", ErrorType.Conflict);

            await _buildings.DeleteAsync(existingBuilding);
            return Result.Success();
        }
        public async Task<ResultT<IReadOnlyList<Building>>> GetAllAsync()
        {
            var buildings = await _buildings.GetAllAsync();
            return ResultT<IReadOnlyList<Building>>.Success(buildings);
        }
        public async Task<PagedResult<Building>> GetAllAsync(BuildingFilter filters)
        {
            var buildings = await _buildings.GetFilteredAsync(filters);
            return PagedResult<Building>.Success(buildings.Buildings, buildings.TotalCount, filters.Page, filters.PageSize);
        }
        public async Task<ResultT<Building>> GetByIdAsync(Guid buildingId)
        {
            var building = await _buildings.GetByIdAsync(buildingId);
            if (building is null)
                return new Error("Building not found", ErrorType.NotFound);

            return ResultT<Building>.Success(building);
        }
        public async Task<ResultT<Building>> UpdateAsync(Guid buildingId, BuildingRequest request)
        {
            var buildingToUpdate = await _buildings.GetByIdAsync(buildingId);
            if (buildingToUpdate is null)
                return new Error("Building not found", ErrorType.NotFound);

            var existingBuilding = await _buildings.GetByNameAsync(request.Name);
            if (existingBuilding is not null && existingBuilding.Id != buildingId)
                return new Error("Building with the same name already exists", ErrorType.Conflict);

            var newAvailabilities = request.Availabilities.Select(a => new Availability
            {
                BuildingId = buildingId,
                DayOfWeek = a.DayOfWeek,
                StartTime = a.StartTime,
                EndTime = a.EndTime
            }).ToList();

            if (!_availabilities.AreAvailabilitiesValid(newAvailabilities))
                return new Error("Invalid availabilities provided", ErrorType.BadRequest);

            buildingToUpdate.Name = request.Name;
            buildingToUpdate.Identifier = request.Identifier;
            buildingToUpdate.Street = request.Street;
            buildingToUpdate.City = request.City;
            buildingToUpdate.PostalCode = request.PostalCode;
            buildingToUpdate.FloorsCount = request.FloorsCount;
            buildingToUpdate.Availabilities = newAvailabilities;

            await _buildings.UpdateAsync(buildingToUpdate);
            return ResultT<Building>.Success(buildingToUpdate);
        }
    }
}
