using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RoomReservation.Api.Attributes;
using RoomReservation.Api.Dtos;
using RoomReservation.Api.Dtos.Buildings.Requests;
using RoomReservation.Api.Dtos.Buildings.Responses;
using RoomReservation.Api.Extensions;
using RoomReservation.Api.Extensions.Mappers;
using RoomReservation.Core.Constants;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Models.Availability;

namespace RoomReservation.Api.Controllers
{

    [EnableRateLimiting("default")]
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class BuildingsController(IBuildingService buildingService) : ControllerBase
    {
        [HttpGet]
        [RequirePermission(Permissions.BuildingList)]
        public async Task<ActionResult<PagedResponseDto<BasicBuildingResponseDto>>> GetFiltered([FromQuery] BuildingFilter filters)
        {
            var result = await buildingService.GetAllAsync(filters);
            return result.ToActionResult(page => Ok(page.ToPagedDto(b => b.ToBasicDto())));
        }

        [HttpGet("{buildingId:guid}")]
        [RequirePermission(Permissions.BuildingView)]
        public async Task<ActionResult<BuildingDetailsResponseDto>> GetSingle([FromRoute] Guid buildingId)
        {
            var result = await buildingService.GetByIdAsync(buildingId);
            return result.ToActionResult(building => Ok(building.ToDetailsDto()));
        }

        [HttpPost]
        [RequirePermission(Permissions.BuildingAdd)]
        public async Task<ActionResult<BasicBuildingResponseDto>> Create([FromBody] BuildingRequestDto request)
        {
            var result = await buildingService.CreateAsync(ToBuildingRequest(request));
            return result.ToActionResult(building =>
                CreatedAtAction(nameof(GetSingle), new { buildingId = building.Id }, building.ToBasicDto()));
        }

        [HttpPut("{buildingId:guid}")]
        [RequirePermission(Permissions.BuildingEdit)]
        public async Task<ActionResult<BasicBuildingResponseDto>> Update([FromRoute] Guid buildingId, [FromBody] BuildingRequestDto request)
        {
            var result = await buildingService.UpdateAsync(buildingId, ToBuildingRequest(request));
            return result.ToActionResult(building => Ok(building.ToBasicDto()));
        }

        [HttpDelete("{buildingId:guid}")]
        [RequirePermission(Permissions.BuildingDelete)]
        public async Task<IActionResult> Delete([FromRoute] Guid buildingId)
        {
            var result = await buildingService.DeleteAsync(buildingId);
            return result.ToActionResult(NoContent);
        }

        private static BuildingModel ToBuildingRequest(BuildingRequestDto request)
        {
            return new BuildingModel(
                Name: request.Name,
                Identifier: request.Identifier,
                Street: request.Street,
                City: request.City,
                PostalCode: request.PostalCode,
                FloorsCount: request.FloorsCount,
                Availabilities: [.. request.Availabilities.Select(a => new AvailabilityModel(
                    DayOfWeek: a.DayOfWeek,
                    StartTime: a.StartTime,
                    EndTime: a.EndTime))]
            );
        }
    }
}
