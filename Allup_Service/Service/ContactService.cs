using Allup_Service.Service.IService;
using Allup_Service.UI.Dtos;
using CloudinaryDotNet.Core;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Allup_Service.Service
{
    public class ContactService : IContactService
    {
        private readonly IEmailService _emailService;
        public ContactService(IEmailService emailService)
        {
            _emailService = emailService;
        }
        public async Task<bool> SendEmailAsync(ContactDto dto, ModelStateDictionary ModelState)
        {
            if (!ModelState.IsValid)
                return false;

            string emailBody = _getTemplateContent(dto);

            await _emailService.SendEmailAsync(new() { Body = emailBody, ToEmail = "info@Allup.az", Subject = "Contact Info" });

            return true;
        }

        private string _getTemplateContent(ContactDto dto)
        {
            string result = @"<!DOCTYPE html>
<html lang=""az"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Əlaqə Sorğusu</title>
    <style>
        body {
            font-family: 'Segoe UI', sans-serif;
            background-color: #f9f9f9;
            margin: 0;
            padding: 0;
        }

        .container {
            max-width: 650px;
            margin: 30px auto;
            background-color: #fff;
            border-radius: 10px;
            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }

        .header {
            background-color: #d32f2f;
            color: #fff;
            padding: 25px;
            text-align: center;
        }

        .header h1 {
            margin: 0;
            font-size: 26px;
        }

        .content {
            padding: 30px;
        }

        .section {
            margin-bottom: 20px;
        }

        .section h3 {
            margin: 0 0 5px;
            color: #333;
            font-size: 16px;
        }

        .section p {
            margin: 0;
            font-size: 15px;
            color: #555;
        }

        .footer {
            text-align: center;
            padding: 15px;
            background-color: #f1f1f1;
            font-size: 13px;
            color: #999;
        }

        .footer a {
            color: #d32f2f;
            text-decoration: none;
        }

        .footer a:hover {
            text-decoration: underline;
        }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>Yeni Əlaqə Sorğusu</h1>
        </div>
        <div class=""content"">
            <div class=""section"">
                <h3>Ad Soyad:</h3>
                <p>[REPLACE_FULLNAME]</p>
            </div>
            <div class=""section"">
                <h3>Email:</h3>
                <p>[REPLACE_EMAIL]</p>
            </div>
            <div class=""section"">
                <h3>Mövzu:</h3>
                <p>[REPLACE_SUBJECT]</p>
            </div>
            <div class=""section"">
                <h3>Mesaj:</h3>
                <p>[REPLACE_MESSAGE]</p>
            </div>
        </div>
        <div class=""footer"">
            Bu mesaj <a href=""https://yourrestaurantdomain.com"">YourAllup_Project</a> vebsaytından göndərilib.
        </div>
    </div>
</body>
</html>";

            result = result.Replace("[REPLACE_FULLNAME]", dto.Name);
            result = result.Replace("[REPLACE_EMAIL]", dto.Email);
            result = result.Replace("[REPLACE_SUBJECT]", dto.Subject ?? "—");
            result = result.Replace("[REPLACE_MESSAGE]", dto.Message);

            return result;
        }

    }
}
