using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RoomReservation.Api.Attributes;
using RoomReservation.Api.Dtos;
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
    public class PermissionsController(IPermissionService permissionService) : ControllerBase
    {
        [HttpGet]
        [RequirePermission(Permissions.PermissionList)]
        public async Task<ActionResult<PagedResponseDto<PermissionResponseDto>>> GetAll([FromQuery] PermissionFilter filters)
        {
            var result = await permissionService.GetAllPermissionsAsync(filters);
            return result.ToActionResult(page => Ok(page.ToPagedDto(p => p.ToDto())));
        }
    }
}
