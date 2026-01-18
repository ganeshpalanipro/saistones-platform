using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saistones.Application.DTOs;
using System.Threading.Tasks;
using Saistones.Domain.Entities;

namespace Saistones.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto?> GetByEmailAsync(string email);
        Task<UserDto> CreateAsync(UserDto user);
        Task<User> RegisterAsync(RegisterUserDto dto);
        Task<User> ValidateUserAsync(LoginUserDto dto);

    }
}
