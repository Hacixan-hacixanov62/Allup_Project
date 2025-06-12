
using Allup_Service.Abstractions.Dtos;

namespace Allup_Service.Dtos.EmailDtos
{
    public class MailKitConfigurationDto:IDto
    {
        public string Mail { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Host { get; set; } = null!;
        public string Port { get; set; } = null!;
    }
}
