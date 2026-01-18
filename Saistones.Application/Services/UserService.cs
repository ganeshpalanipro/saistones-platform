using Saistones.Application.DTOs;
using Saistones.Domain.Entities;
using Saistones.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Saistones.Application.Interfaces;
using System.Threading.Tasks;


namespace Saistones.Application.Services
{
    public class UserService :IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserDto?> GetByEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return null;

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                DisplayName = user.DisplayName
            };
        }


        public async Task<UserDto> CreateAsync(UserDto dto)
        {
            var entity = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                DisplayName = dto.DisplayName
            };

            await _userRepository.AddAsync(entity);

            return new UserDto
            {
                Id = entity.Id,
                Email = entity.Email,
                DisplayName = entity.DisplayName
            };
        }

        public async Task<User> RegisterAsync(RegisterUserDto dto)
        {
            _passwordHasher.CreatePasswordHash(
             dto.Password,
             out var hash,
             out var salt
             );

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = dto.Email,
                DisplayName = dto.DisplayName,
                PasswordHash = hash,
                PasswordSalt = salt,
                IsActive = true
            };

            await _userRepository.AddAsync(user);
            return user;
        }

        public async Task<User> ValidateUserAsync(LoginUserDto dto)
        {
            var user = await _userRepository.GetByEmailAsync(dto.Email);

            if (user == null)
                throw new UnauthorizedAccessException("Invalid credentials");
            // 🔒 BLOCK INACTIVE USERS HERE
            if (!user.IsActive)
                throw new UnauthorizedAccessException("User is inactive");

            var valid = _passwordHasher.VerifyPassword(
                dto.Password,
                user.PasswordHash,
                user.PasswordSalt
            );

            if (!valid)
                throw new UnauthorizedAccessException("Invalid credentials");

            return user;
        }



        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User?> UpdateAsync(Guid id, UpdateUserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return null;

            user.Email = dto.Email;
            user.DisplayName = dto.DisplayName;
            if (dto.IsActive.HasValue)
                user.IsActive = dto.IsActive.Value;


            await _userRepository.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return false;

            user.IsActive = false;
            await _userRepository.SaveChangesAsync();
            return true;
        }

    }
}
