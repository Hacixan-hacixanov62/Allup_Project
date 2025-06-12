

using Allup_Service.Dtos.EmailDtos;
using Allup_Service.Service.IService;

namespace Allup_Service.Service
{
    public class EmailService : IEmailService
    {
        public void SendEmail(string to, string subject, string body)
        {
            throw new NotImplementedException();
        }

        public Task SendEmailAsync(EmailSendDto dto)
        {
            throw new NotImplementedException();
        }
    }
}
