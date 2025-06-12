using Allup_Service.Dtos.AuthDtos;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Service.Validators.AppUserValidators
{
    public class LoginDtoValidators:AbstractValidator<LoginDto>
    {
        public LoginDtoValidators()
        {
            RuleFor(x => x.EmailOrUsername).NotEmpty().MaximumLength(256);
            RuleFor(x => x.Password).NotEmpty().MaximumLength(128);
        }
    }
}
