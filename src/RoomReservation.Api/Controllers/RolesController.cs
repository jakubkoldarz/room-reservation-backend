using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RoomReservation.Api.Attributes;
using RoomReservation.Api.Dtos;
using RoomReservation.Api.Dtos.Roles.Requests;
using RoomReservation.Api.Dtos.Roles.Responses;
using RoomReservation.Api.Extensions;
using RoomReservation.Api.Extensions.Mappers;
using RoomReservation.Core.Constants;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;

namespace RoomReservation.Api.Controllers
{
    [EnableRateLimiting("default")]
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class RolesController(IRoleService roleService) : ControllerBase
    {
        [RequirePermission(Permissions.RoleList)]
        [HttpGet]
        public async Task<ActionResult<PagedResponseDto<RoleResponseDto>>> GetAll([FromQuery] RoleFilter filters)
        {
            var result = await roleService.GetAllAsync(filters);
            return result.ToActionResult(page => Ok(page.ToPagedDto(r => r.ToDto())));
        }

        [RequirePermission(Permissions.RoleView)]
        [HttpGet("{roleId:guid}")]
        public async Task<ActionResult<RoleResponseDto>> GetSingle([FromRoute] Guid roleId)
        {
            var result = await roleService.GetByIdAsync(roleId);
            return result.ToActionResult(role => Ok(role.ToDto()));
        }

        [RequirePermission(Permissions.RoleAdd)]
        [HttpPost]
        public async Task<ActionResult<RoleResponseDto>> Create([FromBody] RoleRequestDto request, [FromQuery] bool force = false)
        {
            var result = await roleService.CreateAsync(ToRoleRequest(request), force);
            return result.ToActionResult(role =>
                CreatedAtAction(nameof(GetSingle), new { roleId = role.Id }, role.ToDto()));
        }

        [RequirePermission(Permissions.RoleEdit)]
        [HttpPut("{roleId:guid}")]
        public async Task<ActionResult<RoleResponseDto>> Update(
            [FromRoute] Guid roleId,
            [FromBody] RoleRequestDto request,
            [FromQuery] bool force = false)
        {
            var result = await roleService.UpdateAsync(roleId, ToRoleRequest(request), force);
            return result.ToActionResult(role => Ok(role.ToDto()));
        }

        [RequirePermission(Permissions.RoleDelete)]
        [HttpDelete("{roleId:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid roleId)
        {
            var result = await roleService.DeleteAsync(roleId);
            return result.ToActionResult(NoContent);
        }

        [RequirePermission(Permissions.RoleAssign)]
        [HttpPost("{roleId:guid}/assign")]
        public async Task<IActionResult> Assign([FromRoute] Guid roleId, [FromQuery] Guid userId, [UserId] Guid requestingUserId)
        {
            var result = await roleService.AssignRoleAsync(roleId, userId, requestingUserId);
            return result.ToActionResult(NoContent);
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
