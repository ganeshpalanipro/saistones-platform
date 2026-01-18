using Saistones.Application.DTOs;
using Saistones.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saistones.Application.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
    }
}
