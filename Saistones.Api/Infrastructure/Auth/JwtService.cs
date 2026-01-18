using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Saistones.Api.Configurations;
using Saistones.Api.Core.Interfaces;
using Saistones.Application.DTOs;
using Saistones.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Saistones.Api.Infrastructure.Auth;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;

    public JwtService(IOptions<JwtSettings> jwtSettings, IConfiguration config)
    {
        _jwtSettings = config.GetSection("JwtSettings").Get<JwtSettings>()!;
    }

    public string GenerateToken(UserDto user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("displayName", user.DisplayName)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)
        );

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
