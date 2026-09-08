using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RoomReservation.Api.Attributes;
using RoomReservation.Api.Dtos.Buildings.Requests;
using RoomReservation.Api.Dtos.Buildings.Responses;
using RoomReservation.Api.Extensions;
using RoomReservation.Api.Extensions.Mappers;
using RoomReservation.Core.Constants;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Models.Availability;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Api.Controllers
{

    [EnableRateLimiting("default")]
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class BuildingsController(IBuildingService _buildingService) : ControllerBase
    {
        [HttpGet]
        [RequirePermission(Permissions.BuildingList)]
        public async Task<ActionResult<PagedResult<BasicBuildingResponseDto>>> GetAll([FromQuery] BuildingFilter filters)
        {
            var result = await _buildingService.GetAllAsync(filters);
            return Ok(result.ToDto(b => b.ToBasicDto()));
        }

        [HttpGet("{buildingId:guid}")]
        [RequirePermission(Permissions.BuildingView)]
        public async Task<ActionResult<BuildingDetailsResponseDto>> GetSingle([FromRoute] Guid buildingId)
        {
            var result = await _buildingService.GetByIdAsync(buildingId);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return Ok(result.Value.ToDetailsDto());
        }

        [HttpPost]
        [RequirePermission(Permissions.BuildingAdd)]
        public async Task<ActionResult<BasicBuildingResponseDto>> Create([FromBody] BuildingRequestDto request)
        {
            var buildingRequest = ToBuildingRequest(request);

            var result = await _buildingService.CreateAsync(buildingRequest);

            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return CreatedAtAction(nameof(GetSingle), new { buildingId = result.Value.Id }, result.Value.ToBasicDto());
        }

        [HttpPut("{buildingId:guid}")]
        [RequirePermission(Permissions.BuildingEdit)]
        public async Task<ActionResult<BasicBuildingResponseDto>> Update([FromRoute] Guid buildingId, [FromBody] BuildingRequestDto request)
        {
            var buildingRequest = ToBuildingRequest(request);

            var result = await _buildingService.UpdateAsync(buildingId, buildingRequest);

            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return Ok(result.Value.ToBasicDto());
        }

        [HttpDelete("{buildingId:guid}")]
        [RequirePermission(Permissions.BuildingDelete)]
        public async Task<IActionResult> Delete([FromRoute] Guid buildingId)
        {
            var result = await _buildingService.DeleteAsync(buildingId);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return NoContent();
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