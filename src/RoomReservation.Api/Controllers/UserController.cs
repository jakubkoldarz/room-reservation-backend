using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomReservation.Api.Dtos.Users.Responses;
using RoomReservation.Api.Extensions;
using RoomReservation.Api.Extensions.Mappers;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UserController(IUserService _userService) : ControllerBase
    {
        [HttpGet("{userId:guid}")]
        public async Task<ActionResult<UserDetailsResponse>> GetSingle(Guid userId)
        {
            var result = await _userService.GetUserDetailsAsync(userId);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();
            
            return Ok(result.Value!.ToDetailsDto());    
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<BasicUserResponse>>> GetAll([FromQuery] UserFilter filters)
        {
            var result = await _userService.GetUsersAsync(filters);
            return Ok(result.ToDto(u => u.ToBasicDto()));
        }
    }
}
