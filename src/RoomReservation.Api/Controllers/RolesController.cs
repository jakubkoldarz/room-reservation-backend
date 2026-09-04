using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomReservation.Api.Attributes;
using RoomReservation.Api.Dtos.Roles.Requests;
using RoomReservation.Api.Dtos.Roles.Responses;
using RoomReservation.Api.Extensions;
using RoomReservation.Api.Extensions.Mappers;
using RoomReservation.Core.Constants;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class RolesController(IRoleService _roleService) : ControllerBase
    {
        [RequirePermission(Permissions.RoleList)]
        [HttpGet]
        public async Task<ActionResult<PagedResult<RoleResponseDto>>> GetAll([FromQuery] RoleFilter filters)
        {
            var result = await _roleService.GetAllAsync(filters);
            return Ok(result.ToDto(r => r.ToDto()));
        }

        [RequirePermission(Permissions.RoleView)]
        [HttpGet("{roleId:guid}")]
        public async Task<ActionResult<RoleResponseDto>> GetSingle([FromRoute] Guid roleId)
        {
            var result = await _roleService.GetByIdAsync(roleId);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return Ok(result.Value.ToDto());
        }

        [RequirePermission(Permissions.RoleAdd)]
        [HttpPost]
        public async Task<ActionResult<RoleResponseDto>> Create([FromBody] RoleRequestDto request, [FromQuery] bool force = false)
        {
            var roleRequest = ToRoleRequest(request);

            var result = await _roleService.CreateAsync(roleRequest, force);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return CreatedAtAction(nameof(GetSingle), new { roleId = result.Value.Id }, result.Value.ToDto());
        }

        [RequirePermission(Permissions.RoleEdit)]
        [HttpPut("{roleId:guid}")]
        public async Task<ActionResult<RoleResponseDto>> Update(
            [FromRoute] Guid roleId,
            [FromBody] RoleRequestDto request,
            [FromQuery] bool force = false)
        {
            var roleRequest = ToRoleRequest(request);

            var result = await _roleService.UpdateAsync(roleId, roleRequest, force);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return Ok(result.Value.ToDto());
        }

        [RequirePermission(Permissions.RoleDelete)]
        [HttpDelete("{roleId:guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid roleId)
        {
            var result = await _roleService.DeleteAsync(roleId);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return NoContent();
        }

        private static RoleModel ToRoleRequest(RoleRequestDto request)
        {
            return new RoleModel(
                Name: request.Name,
                Description: request.Description ?? string.Empty,
                IsDefault: request.IsDefault,
                IsSuperAdmin: request.IsSuperAdmin,
                PermissionIds: request.PermissionIds
            );
        }
    }
}