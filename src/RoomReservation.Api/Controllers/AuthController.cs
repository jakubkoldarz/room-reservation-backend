using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities.Net;
using RoomReservation.Api.Attributes;
using RoomReservation.Api.Dtos;
using RoomReservation.Api.Dtos.Auth.Requests;
using RoomReservation.Api.Dtos.Auth.Responses;
using RoomReservation.Api.Dtos.Users.Responses;
using RoomReservation.Api.Extensions;
using RoomReservation.Api.Extensions.Mappers;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Interfaces;
using System.Net;

namespace RoomReservation.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController(
        IAuthService _authService,
        IUserService _userService,
        IRefreshTokenService _refreshTokenService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<JwtTokenResponse>> Register(RegisterRequest request)
        {
            var (ipAddress, userAgent) = GetUserInfo();

            var result = await _authService.RegisterAsync(request.Email, request.Password, ipAddress, userAgent);

            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            Response.Cookies.AppendRefreshToken(result.Value.RefreshToken);
            return Ok(new JwtTokenResponse(result.Value.JwtToken));
        }

        [Authorize]
        [HttpPost("email/confirmation/verify")]
        public async Task<IActionResult> ConfirmEmail([UserId] Guid userId, VerificationCodedRequest request)
        {
            var result = await _authService.ConfirmEmailAsync(userId, request.VerificationCode);

            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return NoContent();
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            var (ipAddress, userAgent) = GetUserInfo();
            var result = await _authService.LoginAsync(request.Email, request.Password, ipAddress, userAgent);

            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            if (result.Value.Requires2FA)
                return Accepted(new LoginResponse(true, VerificationId: result.Value.VerificationId));

            Response.Cookies.AppendRefreshToken(result.Value.RefreshToken);
            return Ok(new LoginResponse(false, JwtToken: result.Value.JwtToken));
        }

        [HttpPost("login/2fa")]
        public async Task<ActionResult<JwtTokenResponse>> Verify2fa(VerificationRequest request)
        {
            var(ipAddress, userAgent) = GetUserInfo();
            var result = await _authService.Verify2faAsync(
                request.VerificationId,
                request.VerificationCode,
                ipAddress,
                userAgent);

            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            Response.Cookies.AppendRefreshToken(result.Value.RefreshToken);
            return Ok(new JwtTokenResponse(result.Value.JwtToken));
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserDetailsResponse>> Index([UserId] Guid userId)
        {
            var userResult = await _userService.GetUserDetailsAsync(userId);
            if (!userResult.IsSuccess)
                return userResult.Error.ToActionResult();

            var user = userResult.Value!;
            return Ok(user.ToDetailsDto());
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([UserId] Guid userId)
        {
            var cookieExist = Request.Cookies.TryGetValue("refreshToken", out var refreshToken);
            if (!cookieExist || string.IsNullOrEmpty(refreshToken))
                return BadRequest(new ErrorResponse("You are not logged in", HttpStatusCode.BadRequest));

            await _refreshTokenService.RevokeAsync(userId, refreshToken);

            Response.Cookies.DeleteRefreshToken();
            return NoContent();
        }

        [Authorize]
        [HttpDelete("sessions/{refreshTokenId:guid}")]
        public async Task<IActionResult> DeleteRefreshToken([UserId] Guid userId, Guid refreshTokenId)
        {
            var revokeResult = await _refreshTokenService.RevokeAsync(userId, refreshTokenId);
            if (!revokeResult.IsSuccess)
                return revokeResult.Error.ToActionResult();

            return NoContent();
        }

        [Authorize]
        [HttpDelete("sessions")]
        public async Task<IActionResult> DeleteAllRefreshTokens([UserId] Guid userId)
        {
            var revokeResult = await _refreshTokenService.RevokeAllAsync(userId);
            if (!revokeResult.IsSuccess)
                return revokeResult.Error.ToActionResult();

            return NoContent();
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<JwtTokenResponse>> Refresh()
        {
            var cookieExist = Request.Cookies.TryGetValue("refreshToken", out var refreshToken);
            if (!cookieExist || string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new ErrorResponse("You are not logged in", HttpStatusCode.Unauthorized));

            var (ipAddress, userAgent) = GetUserInfo();

            var tokensResponse = await _refreshTokenService.RotateTokenAsync(refreshToken, ipAddress, userAgent);
            if (!tokensResponse.IsSuccess)
                return tokensResponse.Error.ToActionResult();
            Response.Cookies.AppendRefreshToken(tokensResponse.Value.refreshToken);

            return Ok(new JwtTokenResponse(tokensResponse.Value.jwtToken));
        }

        [Authorize]
        [HttpPost("password")]
        public async Task<IActionResult> ChangePassword([UserId] Guid userId, ChangePasswordRequest request)
        {
            var result = await _authService.ChangePasswordAsync(userId, request.OldPassword, request.NewPassword);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return NoContent();
        }

        [Authorize]
        [HttpPost("email")]
        public async Task<ActionResult<VerificationIdResponse>> ChangeEmail([UserId] Guid userId, EmailRequest request)
        {
            var result = await _authService.IssueChangeEmailAsync(userId, request.EmailAddress);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();
         
            return Ok(new VerificationIdResponse(result.Value.Id));
        }

        [Authorize]
        [HttpPost("email/verify")]
        public async Task<IActionResult> ConfirmEmailChange(VerificationRequest request)
        {
            var result = await _authService.ConfirmEmailChangeAsync(request.VerificationId, request.VerificationCode);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return NoContent();
        }

        [Authorize]
        [HttpPost("email/confirmation")]
        public async Task<ActionResult<VerificationIdResponse>> SendEmailConfirmation([UserId] Guid userId)
        {
            var result = await _authService.IssueEmailVerificationAsync(userId);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return Ok(new VerificationIdResponse(result.Value));
        }


        private (string? ipAddress, string? userAgent) GetUserInfo()
        {
            var userAgent = Request.Headers.UserAgent.ToString();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            return (ipAddress, userAgent);
        }
    }
}
