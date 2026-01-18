using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Saistones.Application.DTOs;
using Saistones.Application.Interfaces;
using Saistones.Application.Services;
using Saistones.Domain.Entities;

namespace Saistones.Api.Controllers
{

    [Authorize]
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{email}")]
        public async Task<IActionResult> GetByEmail(string email)
        {
            var user = await _userService.GetByEmailAsync(email);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDto user)
        {
            var created = await _userService.CreateAsync(user);
            return CreatedAtAction(nameof(GetByEmail), new { email = created.Email }, created);
        }

    }
}
