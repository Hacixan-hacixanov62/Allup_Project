

using Allup_Service.Dtos.EmailDtos;

namespace Allup_Service.Service.IService
{
    public interface IEmailService
    {
        void SendEmail(string to, string subject, string body);
        Task SendEmailAsync(EmailSendDto dto);
    }
}
