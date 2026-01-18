using System.IdentityModel.Tokens.Jwt;
using System.Runtime;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Saistones.Application.DTOs;
using Saistones.Application.Interfaces;
using Saistones.Domain.Entities;

namespace Saistones.Infrastructure.Security
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _settings;
        public JwtService(IOptions<JwtSettings> options)
        {
            _settings = options.Value;
        }
        public string GenerateToken(User user)
        {
            var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("displayName", user.DisplayName)
        };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_settings.SecretKey));

            var creds = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
            claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
