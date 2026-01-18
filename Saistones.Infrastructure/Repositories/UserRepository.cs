using Microsoft.EntityFrameworkCore;
using Saistones.Domain.Entities;
using Saistones.Domain.Interfaces;
using Saistones.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saistones.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db)
        {
            _db = db;
        }
        public Task<User?> GetByEmailAsync(string email)
        {
            return _db.Users.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task AddAsync(User user)
        {
            _db.Users.Add(user);
            await _db.SaveChangesAsync();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _db.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }

    }
}
