using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Utilities.Net;
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
        public async Task<ActionResult<VerificationIdResponse>> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request.Email, request.Password);

            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return Accepted(new VerificationIdResponse(result.Value));
        }

        [HttpPost("confirm-email")]
        public async Task<ActionResult<JwtTokenResponse>> ConfirmEmail(VerificationRequest request)
        {
            var (ipAddress, userAgent) = GetUserInfo();

            var result = await _authService.VerifyEmailAsync(
                request.VerificationId,
                request.VerificationCode,
                ipAddress,
                userAgent);

            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            Response.Cookies.AppendRefreshToken(result.Value.refreshToken);
            return Ok(new JwtTokenResponse(result.Value.jwtToken));
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

        [HttpPost("verify-2fa")]
        public async Task<ActionResult<JwtTokenResponse>> Verify2fa(VerificationRequest request)
        {
            var(ipAddress, userAgent) = GetUserInfo();
            var result = await _authService.VerifyLoginCodeAsync(
                request.VerificationId,
                request.VerificationCode,
                ipAddress,
                userAgent);

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
        [HttpPost("logout")]
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
            return NoContent();
        }

        [Authorize]
        [HttpDelete("sessions/{refreshTokenId:guid}")]
        public async Task<ActionResult> DeleteRefreshToken(Guid refreshTokenId)
        {
            var idResult = User.GetId();
            if(!idResult.IsSuccess)
                return idResult.Error.ToActionResult();

            var revokeResult = await _refreshTokenService.RevokeAsync(idResult.Value, refreshTokenId);
            if (!revokeResult.IsSuccess)
                return revokeResult.Error.ToActionResult();

            return NoContent();
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
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

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var idResult = User.GetId();
            if (!idResult.IsSuccess)
                return idResult.Error.ToActionResult();

            var result = await _authService.ChangePasswordAsync(idResult.Value, request.OldPassword, request.NewPassword);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return NoContent();
        }

        [HttpPost("change-email")]
        public async Task<IActionResult> ChangeEmail(EmailRequest request)
        {
            var idResult = User.GetId();
            if (!idResult.IsSuccess)
                return idResult.Error.ToActionResult();

            var result = await _authService.IssueChangeEmailAsync(idResult.Value, request.EmailAddress);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();
         
            return Ok(new VerificationIdResponse(result.Value.Id));
        }

        [HttpPost("verify-change-email")]
        public async Task<IActionResult> VerifyChangeEmail(VerificationRequest request)
        {
            var result = await _authService.VerifyChangedEmailAsync(request.VerificationId, request.VerificationCode);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return NoContent();
        }


        private (string? ipAddress, string? userAgent) GetUserInfo()
        {
            var userAgent = Request.Headers.UserAgent.ToString();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            return (ipAddress, userAgent);
        }
    }
}
