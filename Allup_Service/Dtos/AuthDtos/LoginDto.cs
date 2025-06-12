
using Allup_Service.Abstractions.Dtos;
using System.ComponentModel.DataAnnotations;

namespace Allup_Service.Dtos.AuthDtos
{
    public class LoginDto:IDto
    {
        public string EmailOrUsername { get; set; } = null!;

        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; } = false;
        public string? ReturnUrl { get; set; }
    }
}
