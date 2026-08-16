using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Providers;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public class BuildingService(IBuildingRepository _buildings, IRoomRepository _rooms) : IBuildingService
    {
        public async Task<Result> AddSpecialAvailabilityAsync(Guid buildingId, SpecialAvailabilitySlot specialAvailability)
        {
            if(specialAvailability.StartDate < DateOnly.FromDateTime(DateTime.UtcNow.Date))
                return new Error("Invalid start date", ErrorType.BadRequest);

            if(specialAvailability.IsClosed && (specialAvailability.StartTime is not null || specialAvailability.EndTime is not null))
                return new Error("Cannot specify start or end times for closed availability", ErrorType.BadRequest);

            if (!specialAvailability.IsClosed && (specialAvailability.StartTime is null || specialAvailability.EndTime is null))
                return new Error("Missing start or end time for open availability", ErrorType.BadRequest);

            var building = await _buildings.GetByIdAsync(buildingId);
            if (building is null)
                return new Error("Building not found", ErrorType.NotFound);

            var existingSlots = building.SpecialAvailabilities
                .Where(a => a.EndDate >= DateOnly.FromDateTime(DateTime.UtcNow.Date))
                .Select(a => new SpecialAvailabilitySlot(a.StartDate, a.EndDate, a.IsClosed, a.StartTime, a.EndTime));

            var combinedSlots = existingSlots.Append(specialAvailability).ToList();

            if (!AvailabilityProvider.AreSpecialAvailabilitiesValid(combinedSlots))
                return new Error("Invalid or overlapping special availability", ErrorType.Conflict);

            var specialAvailabilityToCreate = new BuildingSpecialAvailability
            {
                BuildingId = buildingId,
                StartDate = specialAvailability.StartDate,
                EndDate = specialAvailability.EndDate,
                IsClosed = specialAvailability.IsClosed,
                StartTime = specialAvailability.StartTime,
                EndTime = specialAvailability.EndTime
            };

            await _buildings.AddSpecialAvailabilityAsync(specialAvailabilityToCreate);
            return Result.Success();
        }

        public async Task<ResultT<Building>> CreateAsync(string name, string? identifier, string street, string city, string postalCode, int floorsCount, IReadOnlyList<AvailabilitySlot> availabilities)
        {
            var existingBuilding = await _buildings.ExistsByNameAsync(name);
            if (existingBuilding)
                return new Error("Building with the same name already exists", ErrorType.Conflict);

            var validAvailabilities = AvailabilityProvider.AreAvailabilitiesValid(availabilities);
            if (!validAvailabilities)
                return new Error("Invalid availabilities provided", ErrorType.BadRequest);

            var buildingToCreate = new Building
            {
                Name = name,
                Identifier = identifier,
                Street = street,
                City = city,
                PostalCode = postalCode,
                FloorsCount = floorsCount,
                Availabilities = [.. availabilities.Select(a => new BuildingAvailability
                {
                    DayOfWeek = a.DayOfWeek,
                    StartTime = a.StartTime,
                    EndTime = a.EndTime
                })]
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

        public async Task<Result> RemoveSpecialAvailabilityAsync(Guid specialAvailabilityId)
        {
            var deleted = await _buildings.DeleteSpecialAvailabilityByIdAsync(specialAvailabilityId);
            if (!deleted)
                return new Error("Special availability not found", ErrorType.NotFound);

            return Result.Success();
        }

        public async Task<ResultT<Building>> UpdateAsync(Guid buildingId, string name, string? identifier, string street, string city, string postalCode, int floorsCount, IReadOnlyList<AvailabilitySlot> availabilities)
        {
            var buildingToUpdate = await _buildings.GetByIdAsync(buildingId);
            if (buildingToUpdate is null)
                return new Error("Building not found", ErrorType.NotFound);

            var existingBuilding = await _buildings.GetByNameAsync(name);

            if ((existingBuilding is not null) && (existingBuilding.Id != buildingId))
                return new Error("Building with the same name already exists", ErrorType.Conflict);

            var validAvailabilities = AvailabilityProvider.AreAvailabilitiesValid(availabilities);
            if (!validAvailabilities)
                return new Error("Invalid availabilities provided", ErrorType.BadRequest);

            buildingToUpdate.Name = name;
            buildingToUpdate.Identifier = identifier;
            buildingToUpdate.Street = street;
            buildingToUpdate.City = city;
            buildingToUpdate.PostalCode = postalCode;
            buildingToUpdate.FloorsCount = floorsCount;
            buildingToUpdate.Availabilities = [.. availabilities.Select(a => new BuildingAvailability
            {
                DayOfWeek = a.DayOfWeek,
                StartTime = a.StartTime,
                EndTime = a.EndTime
            })];

            await _buildings.UpdateAsync(buildingToUpdate);
            return ResultT<Building>.Success(buildingToUpdate);
        }
    }
}
