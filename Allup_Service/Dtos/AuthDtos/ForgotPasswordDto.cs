
using Allup_Service.Abstractions.Dtos;

namespace Allup_Service.Dtos.AuthDtos
{
    public class ForgotPasswordDto:IDto
    {
        public string EmailOrUsername { get; set; } = null!;
    }
}
