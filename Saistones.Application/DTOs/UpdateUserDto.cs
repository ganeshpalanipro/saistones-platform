using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saistones.Application.DTOs
{
    public class UpdateUserDto
    {
        public string Email { get; set; } = default!;
        public string DisplayName { get; set; } = default!;
        public bool? IsActive { get; set; } // nullable
    }
}
