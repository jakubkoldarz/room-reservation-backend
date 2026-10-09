using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RoomReservation.Api.Attributes;
using RoomReservation.Api.Dtos.Auth.Requests;
using RoomReservation.Api.Dtos.Auth.Responses;
using RoomReservation.Api.Dtos.Users.Responses;
using RoomReservation.Api.Extensions;
using RoomReservation.Api.Extensions.Mappers;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Api.Controllers
{
    [EnableRateLimiting("default")]
    [Route("[controller]")]
    [ApiController]
    public class AuthController(
        IAuthService authService,
        IUserService userService,
        IPermissionService permissionService,
        IRefreshTokenService refreshTokenService,
        IWebHostEnvironment webHostEnvironment) : ControllerBase
    {
        [EnableRateLimiting("strict")]
        [HttpPost("register")]
        public async Task<ActionResult<JwtTokenResponseDto>> Register(RegisterRequestDto request)
        {
            var (ipAddress, userAgent) = GetUserInfo();

            var result = await authService.RegisterAsync(request.Email, request.Password, ipAddress, userAgent);
            return result.ToActionResult(tokens =>
            {
                Response.Cookies.AppendRefreshToken(tokens.RefreshToken, webHostEnvironment.IsDevelopment());
                return Ok(new JwtTokenResponseDto { JwtToken = tokens.JwtToken });
            });
        }

        [EnableRateLimiting("strict")]
        [Authorize]
        [HttpPost("email/confirmation/verify")]
        public async Task<IActionResult> ConfirmEmail([UserId] Guid userId, VerificationCodeRequestDto request)
        {
            var result = await authService.ConfirmEmailAsync(userId, request.VerificationCode);
            return result.ToActionResult(NoContent);
        }

        [EnableRateLimiting("strict")]
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
        {
            var (ipAddress, userAgent) = GetUserInfo();
            var result = await authService.LoginAsync(request.Email, request.Password, ipAddress, userAgent);

            return result.ToActionResult(login =>
            {
                if (login.Requires2FA)
                    return Accepted(new LoginResponseDto { Requires2FA = true, VerificationId = login.VerificationId });

                Response.Cookies.AppendRefreshToken(login.RefreshToken, webHostEnvironment.IsDevelopment());
                return Ok(new LoginResponseDto { Requires2FA = false, JwtToken = login.JwtToken });
            });
        }

        [EnableRateLimiting("strict")]
        [HttpPost("login/2fa")]
        public async Task<ActionResult<JwtTokenResponseDto>> Verify2fa(VerificationRequestDto request)
        {
            var (ipAddress, userAgent) = GetUserInfo();
            var result = await authService.Verify2faAsync(
                request.VerificationId,
                request.VerificationCode,
                ipAddress,
                userAgent);

            return result.ToActionResult(tokens =>
            {
                Response.Cookies.AppendRefreshToken(tokens.RefreshToken, webHostEnvironment.IsDevelopment());
                return Ok(new JwtTokenResponseDto { JwtToken = tokens.JwtToken });
            });
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<ActionResult<UserDetailsResponseDto>> Index([UserId] Guid userId)
        {
            var userResult = await userService.GetUserDetailsAsync(userId);
            if (!userResult.IsSuccess)
                return userResult.Error.ToActionResult();

            var user = userResult.Value;
            var permissionsResult = await permissionService.GetUserPermissionsAsync(userId);
            return permissionsResult.ToActionResult(permissions => Ok(user.ToDetailsDto(permissions)));
        }

        [Authorize]
        [EnableRateLimiting("strict")]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout([UserId] Guid userId)
        {
            var cookieExist = Request.Cookies.TryGetValue(CookiesExtensions.RefreshTokenCookieName, out var refreshToken);
            if (!cookieExist || string.IsNullOrEmpty(refreshToken))
                return Problem(detail: "You are not logged in", statusCode: StatusCodes.Status400BadRequest);

            await refreshTokenService.RevokeAsync(userId, refreshToken);

            Response.Cookies.DeleteRefreshToken(webHostEnvironment.IsDevelopment());
            return NoContent();
        }

        [Authorize]
        [HttpDelete("sessions/{refreshTokenId:guid}")]
        public async Task<IActionResult> DeleteRefreshToken([UserId] Guid userId, Guid refreshTokenId)
        {
            var result = await refreshTokenService.RevokeAsync(userId, refreshTokenId);
            return result.ToActionResult(NoContent);
        }

        [Authorize]
        [HttpDelete("sessions")]
        public async Task<IActionResult> DeleteAllRefreshTokens([UserId] Guid userId)
        {
            var result = await refreshTokenService.RevokeAllAsync(userId);
            return result.ToActionResult(NoContent);
        }

        [EnableRateLimiting("strict")]
        [HttpPost("refresh")]
        public async Task<ActionResult<JwtTokenResponseDto>> Refresh()
        {
            var cookieExist = Request.Cookies.TryGetValue(CookiesExtensions.RefreshTokenCookieName, out var refreshToken);
            if (!cookieExist || string.IsNullOrEmpty(refreshToken))
                return Problem(detail: "You are not logged in", statusCode: StatusCodes.Status401Unauthorized);

            var (ipAddress, userAgent) = GetUserInfo();

            var result = await refreshTokenService.RotateTokenAsync(refreshToken, ipAddress, userAgent);
            return result.ToActionResult(tokens =>
            {
                Response.Cookies.AppendRefreshToken(tokens.refreshToken, webHostEnvironment.IsDevelopment());
                return Ok(new JwtTokenResponseDto { JwtToken = tokens.jwtToken });
            });
        }

        [Authorize]
        [RequireCompletedProfile]
        [EnableRateLimiting("strict")]
        [HttpPatch("password")]
        public async Task<IActionResult> ChangePassword([UserId] Guid userId, ChangePasswordRequestDto request)
        {
            var result = await authService.ChangePasswordAsync(userId, request.OldPassword, request.NewPassword);
            return result.ToActionResult(NoContent);
        }

        [Authorize]
        [RequireCompletedProfile]
        [EnableRateLimiting("strict")]
        [HttpPatch("email")]
        public async Task<ActionResult<VerificationIdResponseDto>> ChangeEmail([UserId] Guid userId, EmailRequestDto request)
        {
            var result = await authService.IssueChangeEmailAsync(userId, request.EmailAddress);
            return result.ToActionResult(code => Ok(new VerificationIdResponseDto { VerificationId = code.Id }));
        }

        [Authorize]
        [RequireCompletedProfile]
        [EnableRateLimiting("strict")]
        [HttpPost("2fa/enable")]
        public async Task<IActionResult> Enable2fa([UserId] Guid userId)
        {
            var result = await authService.Enable2faAsync(userId);
            return result.ToActionResult(NoContent);
        }

        [Authorize]
        [EnableRateLimiting("strict")]
        [RequireCompletedProfile]
        [HttpPost("2fa/disable")]
        public async Task<IActionResult> Disable2fa([UserId] Guid userId, ConfirmPasswordRequestDto request)
        {
            var result = await authService.Disable2faAsync(userId, request.Password);
            return result.ToActionResult(NoContent);
        }

        [Authorize]
        [EnableRateLimiting("strict")]
        [HttpPost("email/verify")]
        public async Task<IActionResult> ConfirmEmailChange(VerificationRequestDto request)
        {
            var result = await authService.ConfirmEmailChangeAsync(request.VerificationId, request.VerificationCode);
            return result.ToActionResult(NoContent);
        }

        [Authorize]
        [EnableRateLimiting("strict")]
        [HttpPost("email/confirmation")]
        public async Task<ActionResult<VerificationIdResponseDto>> SendEmailConfirmation([UserId] Guid userId)
        {
            var result = await authService.IssueEmailVerificationAsync(userId);
            return result.ToActionResult(verificationId => Ok(new VerificationIdResponseDto { VerificationId = verificationId }));
        }

        private (string? ipAddress, string? userAgent) GetUserInfo()
        {
            var userAgent = Request.Headers.UserAgent.ToString();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            return (ipAddress, userAgent);
        }
    }
}
