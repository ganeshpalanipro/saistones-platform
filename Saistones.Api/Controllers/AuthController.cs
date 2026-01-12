using Microsoft.AspNetCore.Mvc;
using Saistones.Api.Core.Interfaces;

namespace Saistones.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;

    public AuthController(IJwtService jwtService)
    {
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public IActionResult Login()
    {
        // TEMP: hardcoded user (we’ll replace with DB later)
        var userId = "1";
        var email = "admin@saistones.com";

        var token = _jwtService.GenerateToken(userId, email);

        return Ok(new
        {
            access_token = token
        });
    }
}
