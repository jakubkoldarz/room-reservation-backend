using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RoomReservation.Api.Attributes;
using RoomReservation.Api.Dtos;
using RoomReservation.Api.Dtos.Equipment.Requests;
using RoomReservation.Api.Dtos.Equipment.Responses;
using RoomReservation.Api.Extensions;
using RoomReservation.Api.Extensions.Mappers;
using RoomReservation.Core.Constants;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Api.Controllers
{
    [EnableRateLimiting("default")]
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class EquipmentController(IEquipmentService equipmentService) : ControllerBase
    {
        [HttpGet]
        [RequirePermission(Permissions.EquipmentList)]
        public async Task<ActionResult<PagedResponseDto<EquipmentResponseDto>>> GetAll([FromQuery] EquipmentFilter filters)
        {
            var result = await equipmentService.GetAllAsync(filters);
            return result.ToActionResult(page => Ok(page.ToPagedDto(eq => eq.ToBasicDto())));
        }

        [HttpGet("{equipmentId:guid}")]
        [RequirePermission(Permissions.EquipmentView)]
        public async Task<ActionResult<EquipmentResponseDto>> GetSingle([FromRoute] Guid equipmentId)
        {
            var result = await equipmentService.GetByIdAsync(equipmentId);
            return result.ToActionResult(equipment => Ok(equipment.ToBasicDto()));
        }

        [HttpPost]
        [RequirePermission(Permissions.EquipmentAdd)]
        public async Task<ActionResult<EquipmentResponseDto>> Create([FromBody] EquipmentRequestDto request)
        {
            var result = await equipmentService.CreateAsync(request.Name, request.Icon);
            return result.ToActionResult(equipment =>
                CreatedAtAction(nameof(GetSingle), new { equipmentId = equipment.Id }, equipment.ToBasicDto()));
        }

        [HttpPut("{equipmentId:guid}")]
        [RequirePermission(Permissions.EquipmentEdit)]
        public async Task<ActionResult<EquipmentResponseDto>> Update([FromRoute] Guid equipmentId, [FromBody] EquipmentRequestDto request)
        {
            var result = await equipmentService.UpdateAsync(equipmentId, request.Name, request.Icon);
            return result.ToActionResult(equipment => Ok(equipment.ToBasicDto()));
        }

        [HttpDelete("{equipmentId:guid}")]
        [RequirePermission(Permissions.EquipmentDelete)]
        public async Task<IActionResult> Delete([FromRoute] Guid equipmentId)
        {
            var result = await equipmentService.DeleteAsync(equipmentId);
            return result.ToActionResult(NoContent);
        }
    }
}
