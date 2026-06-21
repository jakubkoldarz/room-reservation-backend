using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomReservation.Api.Dtos;
using RoomReservation.Api.Dtos.Auth.Requests;
using RoomReservation.Api.Dtos.Auth.Responses;
using RoomReservation.Api.Dtos.Users.Responses;
using RoomReservation.Api.Extensions;
using RoomReservation.Api.Extensions.Mappers;
using RoomReservation.Core.Interfaces;
using System.Net;

namespace RoomReservation.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(IAuthService _authService, IUserService _userService, IRefreshTokenService _refreshTokenService) : ControllerBase
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
            var userAgent = Request.Headers.UserAgent.ToString();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var result = await _authService.LoginAsync(request.Email, request.Password, ipAddress, userAgent);

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
            var cookieExist = Request.Cookies.TryGetValue("refreshToken", out var refreshToken);
            if (!cookieExist || string.IsNullOrEmpty(refreshToken)) 
                return BadRequest(new ErrorResponse("You are not logged in", HttpStatusCode.BadRequest));

            var idResult = User.GetId();
            if (!idResult.IsSuccess) 
                return idResult.Error.ToActionResult();

            await _refreshTokenService.RevokeAsync(idResult.Value, refreshToken);

            Response.Cookies.DeleteRefreshToken();
            return Ok();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var cookieExist = Request.Cookies.TryGetValue("refreshToken", out var refreshToken);
            if (!cookieExist || string.IsNullOrEmpty(refreshToken))
                return BadRequest(new ErrorResponse("You are not logged in", HttpStatusCode.Unauthorized));

            var userAgent = Request.Headers.UserAgent.ToString();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var tokensResponse = await _refreshTokenService.RotateTokenAsync(refreshToken, ipAddress, userAgent);
            if (!tokensResponse.IsSuccess)
                return tokensResponse.Error.ToActionResult();
            Response.Cookies.AppendRefreshToken(tokensResponse.Value.refreshToken);
            
            return Ok(new JwtTokenResponse(tokensResponse.Value.jwtToken));
        }
    }
}
