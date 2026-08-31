using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public class BuildingService(
        IBuildingRepository _buildings,
        IRoomRepository _rooms,
        IAvailabilityService _availabilityService) : IBuildingService
    {
        public async Task<ResultT<Building>> CreateAsync(BuildingModel model)
        {
            var existingBuilding = await _buildings.ExistsByNameAsync(model.Name);
            if (existingBuilding)
                return new Error("Building with the same name already exists", ErrorType.Conflict);

            var availabilityEntities = model.Availabilities.Select(a => new Availability
            {
                DayOfWeek = a.DayOfWeek,
                StartTime = a.StartTime,
                EndTime = a.EndTime
            }).ToList();

            var validationResult = await _availabilityService.AreAvailabilitiesValid(availabilityEntities);
            if (!validationResult.IsSuccess)
                return validationResult.Error;

            var buildingToCreate = new Building
            {
                Name = model.Name,
                Identifier = model.Identifier,
                Street = model.Street,
                City = model.City,
                PostalCode = model.PostalCode,
                FloorsCount = model.FloorsCount,
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
        public async Task<ResultT<Building>> UpdateAsync(Guid buildingId, BuildingModel model)
        {
            var buildingToUpdate = await _buildings.GetByIdAsync(buildingId);
            if (buildingToUpdate is null)
                return new Error("Building not found", ErrorType.NotFound);

            var existingBuilding = await _buildings.GetByNameAsync(model.Name);
            if (existingBuilding is not null && existingBuilding.Id != buildingId)
                return new Error("Building with the same name already exists", ErrorType.Conflict);

            buildingToUpdate.Name = model.Name;
            buildingToUpdate.Identifier = model.Identifier;
            buildingToUpdate.Street = model.Street;
            buildingToUpdate.City = model.City;
            buildingToUpdate.PostalCode = model.PostalCode;
            buildingToUpdate.FloorsCount = model.FloorsCount;
            
            var replacementResult = await _availabilityService.ReplaceIfValidForBuildingAsync(buildingToUpdate, model.Availabilities);
            if (!replacementResult.IsSuccess)
                return replacementResult.Error;

            await _buildings.UpdateAsync(buildingToUpdate);
            return ResultT<Building>.Success(buildingToUpdate);
        }
    }
}
