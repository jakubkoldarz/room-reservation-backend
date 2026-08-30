using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomReservation.Api.Attributes;
using RoomReservation.Api.Dtos.Permissions.Responses;
using RoomReservation.Core.Constants;
using RoomReservation.Core.Services;

namespace RoomReservation.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class PermissionsController(PermissionService _permissionService) : ControllerBase
    {
        [HttpGet]
        [RequirePermission(Permissions.PermissionList)]
        public async Task<ActionResult<IReadOnlyList<PermissionResponseDto>>> GetAll()
        {
            var result = await _permissionService.GetAllPermissionsAsync();
            return Ok(result);
        }
    }
}
