using System.Security.Claims;
using SrijanDEEP.API.DTOs;
using SrijanDEEP.API.Entities;
using SrijanDEEP.API.Helpers;
using SrijanDEEP.API.Repositories;

namespace SrijanDEEP.API.Services;

public interface IAuthService
{
    Task<(bool Success, string Message, LoginResponseDto? Data)> LoginAsync(LoginRequestDto request);
    Task<(bool Success, string Message, RefreshTokenResponseDto? Data)> RefreshTokenAsync(RefreshTokenRequestDto request);
    Task<(bool Success, string Message)> ForgotPasswordAsync(ForgotPasswordRequestDto request);
    Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordRequestDto request);
    Task<(bool Success, string Message)> ChangePasswordAsync(int userId, ChangePasswordRequestDto request);
}

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;

    // In-memory store for password-reset tokens. In production this should be a
    // persisted, expiring store (e.g. a PasswordResetTokens table or distributed cache)
    // and the token should be emailed to the user rather than returned in any response.
    private static readonly Dictionary<string, (string Token, DateTime Expiry)> ResetTokens = new();

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
    }

    public async Task<(bool Success, string Message, LoginResponseDto? Data)> LoginAsync(LoginRequestDto request)
    {
        
        var user = await _userRepository.GetByUsernameAsync(request.Username);

        Console.WriteLine($"User Found: {user != null}");

        if (user != null)
        {
            Console.WriteLine($"Username: {user.Username}");
            Console.WriteLine($"Hash: {user.PasswordHash}");
            Console.WriteLine($"Verify: {PasswordHasher.Verify(request.Password, user.PasswordHash)}");
        }

        if (user is null || !user.IsActive || !PasswordHasher.Verify(request.Password, user.PasswordHash))
        {
            return (false, "Invalid username or password.", null);
        }

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshTokenValue = _tokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            UserId = user.UserId,
            Token = refreshTokenValue,
            ExpiryDate = _tokenService.GetRefreshTokenExpiry(),
            IsRevoked = false
        };

        await _refreshTokenRepository.AddAsync(refreshToken);
        await _refreshTokenRepository.SaveChangesAsync();

        var response = new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            AccessTokenExpiry = _tokenService.GetAccessTokenExpiry(),
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role
        };

        return (true, "Login successful.", response);
    }

    public async Task<(bool Success, string Message, RefreshTokenResponseDto? Data)> RefreshTokenAsync(RefreshTokenRequestDto request)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal is null)
        {
            return (false, "Invalid access token.", null);
        }

        var userIdClaim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userIdClaim is null || !int.TryParse(userIdClaim, out var userId))
        {
            return (false, "Invalid access token.", null);
        }

        var storedToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
        if (storedToken is null || storedToken.UserId != userId || !storedToken.IsActive)
        {
            return (false, "Invalid or expired refresh token.", null);
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null || !user.IsActive)
        {
            return (false, "User account is not active.", null);
        }

        // Rotate the refresh token: revoke the old one, issue a new one.
        storedToken.IsRevoked = true;
        _refreshTokenRepository.Update(storedToken);

        var newAccessToken = _tokenService.GenerateAccessToken(user);
        var newRefreshTokenValue = _tokenService.GenerateRefreshToken();

        await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            UserId = user.UserId,
            Token = newRefreshTokenValue,
            ExpiryDate = _tokenService.GetRefreshTokenExpiry(),
            IsRevoked = false
        });

        await _refreshTokenRepository.SaveChangesAsync();

        return (true, "Token refreshed successfully.", new RefreshTokenResponseDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenValue,
            AccessTokenExpiry = _tokenService.GetAccessTokenExpiry()
        });
    }

    public async Task<(bool Success, string Message)> ForgotPasswordAsync(ForgotPasswordRequestDto request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        // Always return the same generic message, whether or not the email exists,
        // so the endpoint can't be used to enumerate registered accounts.
        const string genericMessage = "If an account with that email exists, password reset instructions have been sent.";

        if (user is null || !user.IsActive)
        {
            return (true, genericMessage);
        }

        var resetToken = Guid.NewGuid().ToString("N");
        ResetTokens[request.Email] = (resetToken, DateTime.UtcNow.AddMinutes(30));

        // TODO: integrate an email provider and send `resetToken` as a link instead
        // of relying on this in-memory placeholder.

        return (true, genericMessage);
    }

    public async Task<(bool Success, string Message)> ResetPasswordAsync(ResetPasswordRequestDto request)
    {
        if (!ResetTokens.TryGetValue(request.Email, out var entry) ||
            entry.Token != request.ResetToken ||
            entry.Expiry < DateTime.UtcNow)
        {
            return (false, "Invalid or expired reset token.");
        }

        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user is null)
        {
            return (false, "Invalid or expired reset token.");
        }

        user.PasswordHash = PasswordHasher.Hash(request.NewPassword);
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        ResetTokens.Remove(request.Email);

        return (true, "Password has been reset successfully.");
    }

    public async Task<(bool Success, string Message)> ChangePasswordAsync(int userId, ChangePasswordRequestDto request)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user is null || !user.IsActive)
        {
            return (false, "User not found.");
        }

        if (!PasswordHasher.Verify(request.CurrentPassword, user.PasswordHash))
        {
            return (false, "Current password is incorrect.");
        }

        user.PasswordHash = PasswordHasher.Hash(request.NewPassword);
        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();

        return (true, "Password changed successfully.");
    }
}