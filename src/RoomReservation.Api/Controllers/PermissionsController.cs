using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RoomReservation.Api.Attributes;
using RoomReservation.Api.Dtos.Permissions.Responses;
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
    public class PermissionsController(IPermissionService _permissionService) : ControllerBase
    {
        [HttpGet]
        [RequirePermission(Permissions.PermissionList)]
        public async Task<ActionResult<IReadOnlyList<PermissionResponseDto>>> GetAll([FromQuery] PermissionFilter filters)
        {
            var result = await _permissionService.GetAllPermissionsAsync(filters);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return Ok(result.ToDto(p => p.ToDto()));
        }
    }
}
