using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomReservation.Api.Attributes;
using RoomReservation.Api.Dtos.Equipment.Requests;
using RoomReservation.Api.Dtos.Equipment.Responses;
using RoomReservation.Api.Extensions;
using RoomReservation.Api.Extensions.Mappers;
using RoomReservation.Core.Constants;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class EquipmentController(IEquipmentService _equipmentService) : ControllerBase
    {
        [HttpGet]
        [RequirePermission(Permissions.EquipmentList)]
        public async Task<ActionResult<PagedResult<BasicEquipmentResponse>>> GetAll([FromQuery] EquipmentFilter filters)
        {
            var result = await _equipmentService.GetAllAsync(filters);
            return Ok(result.ToDto(eq => eq.ToBasicDto()));
        }

        [HttpGet("{equipmentId:guid}")]
        [RequirePermission(Permissions.EquipmentView)]
        public async Task<ActionResult<BasicEquipmentResponse>> GetSingle([FromRoute] Guid equipmentId)
        {
            var result = await _equipmentService.GetByIdAsync(equipmentId);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return Ok(result.Value.ToBasicDto());
        }

        [HttpPost]
        [RequirePermission(Permissions.EquipmentAdd)]
        public async Task<ActionResult<BasicEquipmentResponse>> Create([FromBody] EquipmentRequest request)
        {
            var result = await _equipmentService.CreateAsync(request.Name, request.Icon);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return CreatedAtAction(nameof(GetSingle), new { equipmentId = result.Value.Id }, result.Value.ToBasicDto());
        }

        [HttpPut("{equipmentId:guid}")]
        [RequirePermission(Permissions.EquipmentEdit)]
        public async Task<ActionResult<BasicEquipmentResponse>> Update([FromRoute] Guid equipmentId, [FromBody] EquipmentRequest request)
        {
            var result = await _equipmentService.UpdateAsync(equipmentId, request.Name, request.Icon);
            if(!result.IsSuccess)
                return result.Error.ToActionResult();

            return Ok(result.Value.ToBasicDto());
        }

        [HttpDelete("{equipmentId:guid}")]
        [RequirePermission(Permissions.EquipmentDelete)]
        public async Task<ActionResult> Delete([FromRoute] Guid equipmentId)
        {
            var result = await _equipmentService.DeleteAsync(equipmentId);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return NoContent();
        }
    }
}
