using Microsoft.AspNetCore.Mvc;
using Saistones.Api.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Saistones.Application.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Saistones.Application.Interfaces;
using System.Text;
using Saistones.Application.DTOs;
using Saistones.Infrastructure.Security;
using Saistones.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Saistones.Infrastructure.Data;

namespace Saistones.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly IUserService _userService;
    private readonly AppDbContext _context;

    public AuthController(IUserService userService, IJwtService jwtService, AppDbContext context)
    {
        _userService = userService;
        _jwtService = jwtService;
        _context = context;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
    {
        var user = await _userService.ValidateUserAsync(dto);
        if (user == null)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password"
            });
        }

        //  Generate access token (JWT)
        var token = _jwtService.GenerateToken(user);

        //  Generate refresh token value
        var refreshTokenValue = RefreshTokenGenerator.Generate();

        //  Create refresh token entity
        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = refreshTokenValue,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        // Save refresh token in DB
        _context.RefreshTokens.Add(refreshToken);
        await _context.SaveChangesAsync();

        // 5️⃣ Return both tokens
        return Ok(new LoginResponse
        {
            AccessToken = token,
            RefreshToken = refreshTokenValue
        });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto dto)
    {
        var existing = await _userService.GetByEmailAsync(dto.Email);
        if (existing != null)
            return BadRequest("User already exists");

        var user = await _userService.RegisterAsync(dto);

        var token = _jwtService.GenerateToken(user);

        return Ok(new
        {
            accessToken = token
        });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var existingToken = await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        if (existingToken == null ||
            existingToken.IsRevoked ||
            existingToken.ExpiresAt <= DateTime.UtcNow)
        {
            return Unauthorized("Invalid refresh token");
        }

        // Revoke old token
        existingToken.IsRevoked = true;

        // Generate new tokens
        var newAccessToken = _jwtService.GenerateToken(existingToken.User);
        var newRefreshTokenValue = RefreshTokenGenerator.Generate();

        var newRefreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            Token = newRefreshTokenValue,
            UserId = existingToken.UserId,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false,
            ReplacedByToken = newRefreshTokenValue
        };

        // Save changes
        _context.RefreshTokens.Add(newRefreshToken);
        await _context.SaveChangesAsync();

        // Return new tokens
        return Ok(new LoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenValue
        });
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
    {
        var token = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        if (token == null || token.IsRevoked)
        {
            return Ok(); // idempotent logout
        }

        token.IsRevoked = true;
        await _context.SaveChangesAsync();

        return Ok();
    }


}
