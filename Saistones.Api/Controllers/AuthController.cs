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

namespace Saistones.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly IUserService _userService;

    public AuthController(IUserService userService, IJwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
    {
        var user = await _userService.ValidateUserAsync(dto);
        if (user == null)
            return Unauthorized("User not found");

        var token = _jwtService.GenerateToken(user);

        return Ok(new
        {
            accessToken = token
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

}
