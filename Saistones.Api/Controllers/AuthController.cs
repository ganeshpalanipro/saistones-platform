using Microsoft.AspNetCore.Mvc;
using Saistones.Api.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using Saistones.Application.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Saistones.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly UserService _userService;

    public AuthController(UserService userService, IJwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] string email)
    {
        var user = await _userService.GetByEmailAsync(email);
        if (user == null)
            return Unauthorized("User not found");

        var token = _jwtService.GenerateToken(user);

        return Ok(new
        {
            accessToken = token
        });
    }
}
