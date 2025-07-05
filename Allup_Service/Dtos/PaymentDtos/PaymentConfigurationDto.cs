

using Allup_Service.Abstractions.Dtos;

namespace Allup_Service.Dtos.PaymentDtos
{
    public class PaymentConfigurationDto:IDto
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string BaseUrl { get; set; } = null!;
    }
}
