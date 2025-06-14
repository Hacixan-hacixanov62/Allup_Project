using MailKit.Net.Smtp;
using Allup_Service.Dtos.EmailDtos;
using Allup_Service.Service.IService;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using System.Net.Mail;

namespace Allup_Service.Service
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly MailKitConfigurationDto _configurationDto;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _configurationDto = _configuration.GetSection("MailkitOptions").Get<MailKitConfigurationDto>() ?? new();

        }

        public async Task SendEmailAsync(EmailSendDto dto)
        {
            var email = new MimeMessage();

            email.Sender = MailboxAddress.Parse(_configurationDto.Mail);
            email.To.Add(MailboxAddress.Parse(dto.ToEmail));

            email.Subject = dto.Subject;




            var builder = new BodyBuilder();


            if (dto.Attachments != null && dto.Attachments.Count > 0)
            {
                foreach (var attachment in dto.Attachments)
                {
                    if (attachment.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            await attachment.CopyToAsync(ms);
                            builder.Attachments.Add(attachment.FileName, ms.ToArray(), ContentType.Parse(attachment.ContentType));
                        }
                    }
                }
            }

            builder.HtmlBody = dto.Body;
            email.Body = builder.ToMessageBody();

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                // Bypass certificate validation for development/testing purposes
                smtp.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;

                smtp.Connect(_configurationDto.Host, int.Parse(_configurationDto.Port), SecureSocketOptions.StartTls);
                smtp.Authenticate(_configurationDto.Mail, _configurationDto.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
        }

        public void SendEmail(string to, string subject, string body)
        {
            // create email
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse("hajikhanih@code.edu.az"));
                email.To.Add(MailboxAddress.Parse(to));
                email.Subject = subject;
                email.Body = new TextPart(TextFormat.Html) { Text = body };

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate("hajikhanih@code.edu.az", "ffce mabe gven kigb"); // <== App paswords yazmaq
                smtp.Send(email);
                smtp.Disconnect(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }



            //////var bodyBuilder = new BodyBuilder();
            //////bodyBuilder.HtmlBody = body;
            //////email.Body = bodyBuilder.ToMessageBody();
        }

    }
}

