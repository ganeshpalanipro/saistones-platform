
using Saistones.Application.DTOs;
using Saistones.Domain.Entities;

namespace Saistones.Api.Core.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(UserDto user);
    }
}
