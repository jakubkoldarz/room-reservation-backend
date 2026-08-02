using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomReservation.Api.Attributes;
using RoomReservation.Api.Dtos.Buildings.Requests;
using RoomReservation.Api.Dtos.Buildings.Responses;
using RoomReservation.Api.Extensions;
using RoomReservation.Api.Extensions.Mappers;
using RoomReservation.Core.Constants;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Api.Controllers
{
    [Route("[controller]")]
    [Authorize]
    [ApiController]
    public class BuildingController(IBuildingService _buildingService) : ControllerBase
    {
        [HttpGet]
        [RequirePermission(Permissions.BuildingList)]
        public async Task<ActionResult<BasicBuildingResponse[]>> GetAll([FromQuery] BuildingFilter filters)
        {
            var result = await _buildingService.GetAllAsync(filters);
            return Ok(result.ToDto(b => b.ToBasicDto()));
        }

        [HttpGet("{buildingId:guid}")]
        [RequirePermission(Permissions.BuildingView)]
        public async Task<ActionResult<BuildingDetailsResponse>> GetSingle([FromRoute] Guid buildingId)
        {
            var result = await _buildingService.GetByIdAsync(buildingId);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return Ok(result.Value.ToDetailsDto());
        }

        [HttpPost]
        [RequirePermission(Permissions.BuildingAdd)]
        public async Task<ActionResult<BasicBuildingResponse>> Add([FromBody] BuildingRequest request)
        {
            var result = await _buildingService.CreateAsync(
                request.Name,
                request.Identifier,
                request.Street,
                request.City,
                request.PostalCode,
                request.FloorsCount
            );

            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return CreatedAtAction(nameof(GetSingle), new { buildingId = result.Value.Id }, result.Value.ToBasicDto());
        }

        [HttpPut("{buildingId:guid}")]
        [RequirePermission(Permissions.BuildingEdit)]
        public async Task<ActionResult<BasicBuildingResponse>> Update([FromRoute] Guid buildingId, [FromBody] BuildingRequest request)
        {
            var result = await _buildingService.UpdateAsync(
                buildingId,
                request.Name,
                request.Identifier,
                request.Street,
                request.City,
                request.PostalCode,
                request.FloorsCount
            );

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
    }
}
