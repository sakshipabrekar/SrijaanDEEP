using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SrijanDEEP.API.Common;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Services;

namespace SrijanDEEP.API.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Authenticates a user and issues an access token + refresh token.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        var (success, message, data) = await _authService.LoginAsync(request);
        if (!success)
        {
            return Unauthorized(ApiResponse<LoginResponseDto>.FailureResponse(message));
        }

        return Ok(ApiResponse<LoginResponseDto>.SuccessResponse(data, message));
    }

    /// <summary>Exchanges a valid (possibly expired) access token + refresh token for a new pair.</summary>
    [HttpPost("refresh-token")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<RefreshTokenResponseDto>>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        var (success, message, data) = await _authService.RefreshTokenAsync(request);
        if (!success)
        {
            return Unauthorized(ApiResponse<RefreshTokenResponseDto>.FailureResponse(message));
        }

        return Ok(ApiResponse<RefreshTokenResponseDto>.SuccessResponse(data, message));
    }

    /// <summary>Requests a password reset token for the supplied email (sent out-of-band in production).</summary>
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
    {
        var (success, message) = await _authService.ForgotPasswordAsync(request);
        return Ok(success ? ApiResponse.Ok(message) : ApiResponse.Fail(message));
    }

    /// <summary>Completes a password reset using the token issued by forgot-password.</summary>
    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<object>>> ResetPassword([FromBody] ResetPasswordRequestDto request)
    {
        var (success, message) = await _authService.ResetPasswordAsync(request);
        return success ? Ok(ApiResponse.Ok(message)) : BadRequest(ApiResponse.Fail(message));
    }

    /// <summary>Changes the current authenticated user's password.</summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<object>>> ChangePassword([FromBody] ChangePasswordRequestDto request)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiResponse.Fail("Invalid user context."));
        }

        var (success, message) = await _authService.ChangePasswordAsync(userId, request);
        return success ? Ok(ApiResponse.Ok(message)) : BadRequest(ApiResponse.Fail(message));
    }

    [HttpGet("generate-hash")]
    public IActionResult GenerateHash()
    {
        var hash = BCrypt.Net.BCrypt.HashPassword("Admin@123");
        return Ok(hash);
    }
}