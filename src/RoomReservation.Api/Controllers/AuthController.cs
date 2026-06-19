using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomReservation.Api.Dtos.Auth.Requests;
using RoomReservation.Api.Dtos.Auth.Responses;
using RoomReservation.Api.Dtos.Users.Responses;
using RoomReservation.Api.Extensions;
using RoomReservation.Api.Extensions.Mappers;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IAuthService _authService, IUserService _userService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<JwtTokenResponse>> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request.Email, request.Password, request.Firstname, request.Lastname);

            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            Response.Cookies.AppendRefreshToken(result.Value.refreshToken);
            return Ok(new JwtTokenResponse(result.Value.jwtToken));
        }

        [HttpPost("login")]
        public async Task<ActionResult<JwtTokenResponse>> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request.Email, request.Password);

            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            Response.Cookies.AppendRefreshToken(result.Value.refreshToken);
            return Ok(new JwtTokenResponse(result.Value.jwtToken));
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserDetailsResponse>> Index()
        {
            var idResult = User.GetId();
            if (!idResult.IsSuccess)
                return idResult.Error.ToActionResult();

            var userResult = await _userService.GetUserDetailsAsync(idResult.Value);
            if (!userResult.IsSuccess)
                return userResult.Error.ToActionResult();

            var user = userResult.Value!;
            return Ok(user.ToDetailsDto());
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.DeleteRefreshToken();
            return Ok();
        }
    }
}
