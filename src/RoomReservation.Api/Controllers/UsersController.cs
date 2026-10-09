using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RoomReservation.Api.Attributes;
using RoomReservation.Api.Dtos;
using RoomReservation.Api.Dtos.Users.Requests;
using RoomReservation.Api.Dtos.Users.Responses;
using RoomReservation.Api.Extensions;
using RoomReservation.Api.Extensions.Mappers;
using RoomReservation.Core.Constants;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Api.Controllers
{
    [Authorize]
    [EnableRateLimiting("default")]
    [ApiController]
    [Route("[controller]")]
    public class UsersController(IUserService userService) : ControllerBase
    {
        [HttpGet("{userId:guid}")]
        [RequirePermission(Permissions.UserView)]
        public async Task<ActionResult<BasicUserResponseDto>> GetSingle(Guid userId)
        {
            var result = await userService.GetUserDetailsAsync(userId);
            return result.ToActionResult(user => Ok(user.ToBasicDto()));
        }

        [HttpGet]
        [RequirePermission(Permissions.UserList)]
        public async Task<ActionResult<PagedResponseDto<BasicUserResponseDto>>> GetAll([FromQuery] UserFilter filters)
        {
            var result = await userService.GetAllAsync(filters);
            return result.ToActionResult(page => Ok(page.ToPagedDto(u => u.ToBasicDto())));
        }

        [HttpPut("profile")]
        public async Task<ActionResult<BasicUserResponseDto>> UpdateProfile([UserId] Guid userId, UpdateProfileRequestDto request)
        {
            var result = await userService.UpdateUserAsync(userId, request.Firstname, request.Lastname);
            return result.ToActionResult(user => Ok(user.ToBasicDto()));
        }
    }
}
