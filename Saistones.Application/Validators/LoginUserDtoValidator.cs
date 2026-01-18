using FluentValidation;
using Saistones.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saistones.Application.Validators
{
    public class LoginUserDtoValidator: AbstractValidator<LoginUserDto>

    {
        public LoginUserDtoValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty().MinimumLength(6);
        }
    }
}
