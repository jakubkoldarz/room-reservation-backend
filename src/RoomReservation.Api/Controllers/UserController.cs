using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomReservation.Api.Attributes;
using RoomReservation.Api.Dtos.Users.Requests;
using RoomReservation.Api.Dtos.Users.Responses;
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
    public class UserController(IUserService _userService) : ControllerBase
    {
        [HttpGet("{userId:guid}")]
        [RequirePermission(Permissions.UserView)]
        public async Task<ActionResult<UserDetailsResponse>> GetSingle(Guid userId)
        {
            var result = await _userService.GetUserDetailsAsync(userId);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();
            
            return Ok(result.Value!.ToBasicDto());    
        }

        [HttpGet]
        [RequirePermission(Permissions.UserList)]
        public async Task<ActionResult<IEnumerable<BasicUserResponse>>> GetAll([FromQuery] UserFilter filters)
        {
            var result = await _userService.GetUsersAsync(filters);
            return Ok(result.ToDto(u => u.ToBasicDto()));
        }

        [HttpPut("profile")]
        public async Task<ActionResult<IEnumerable<BasicUserResponse>>> UpdateProfile([UserId] Guid userId, UpdateProfileRequest request)
        {
            var result = await _userService.UpdateUserAsync(userId, request.Firstname, request.Lastname);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return Ok(result.Value.ToBasicDto());
        }
    }
}
