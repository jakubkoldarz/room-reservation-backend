using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public class BuildingService(IBuildingRepository _buildings) : IBuildingService
    {
        public async Task<ResultT<Building>> CreateAsync(string name, string? identifier, string street, string city, string postalCode, int floorsCount)
        {
            var existingBuilding = await _buildings.ExistsByNameAsync(name);
            if (existingBuilding)
                return new Error("Building with the same name already exists", ErrorType.Conflict);

            var buildingToCreate = new Building
            {
                Name = name,
                Identifier = identifier,
                Street = street,
                City = city,
                PostalCode = postalCode,
                FloorsCount = floorsCount
            };
            await _buildings.AddAsync(buildingToCreate);
            return ResultT<Building>.Success(buildingToCreate);
        }
        public async Task<Result> DeleteAsync(Guid buildingId)
        {
            var existingBuilding = await _buildings.GetByIdAsync(buildingId);
            if (existingBuilding is null)
                return new Error("Building not found", ErrorType.NotFound);

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
        public async Task<ResultT<Building>> UpdateAsync(Guid buildingId, string name, string? identifier, string street, string city, string postalCode, int floorsCount)
        {
            var buildingToUpdate = await _buildings.GetByIdAsync(buildingId);
            if (buildingToUpdate is null)
                return new Error("Building not found", ErrorType.NotFound);

            var existingBuilding = await _buildings.GetByNameAsync(name);

            if ((existingBuilding is not null) && (existingBuilding.Id != buildingId))
                return new Error("Building with the same name already exists", ErrorType.Conflict);

            buildingToUpdate.Name = name;
            buildingToUpdate.Identifier = identifier;
            buildingToUpdate.Street = street;
            buildingToUpdate.City = city;
            buildingToUpdate.PostalCode = postalCode;
            buildingToUpdate.FloorsCount = floorsCount;

            await _buildings.UpdateAsync(buildingToUpdate);
            return ResultT<Building>.Success(buildingToUpdate);
        }
    }
}
