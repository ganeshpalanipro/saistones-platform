using Saistones.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saistones.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }

        public string Email { get; set; } = null!;
        public string DisplayName { get; set; } = null!;

        public byte[] PasswordHash { get; set; } = null!;
        public byte[] PasswordSalt { get; set; } = null!;

        public bool IsActive { get; set; }
        public UserRole Role { get; set; } = UserRole.User;
    }
}
