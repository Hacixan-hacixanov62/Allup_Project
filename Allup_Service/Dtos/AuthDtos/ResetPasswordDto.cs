

using Allup_Service.Abstractions.Dtos;

namespace Allup_Service.Dtos.AuthDtos
{
    public class ResetPasswordDto:IDto
    {
        public string Token { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
    }
}
