
using Microsoft.AspNetCore.Http;

namespace Allup_Service.Extensions
{
    public static class ExtensionMethods
    {
        public static string GetReturnUrl(this HttpRequest Request)
        {
            string? returnUrl = Request.Headers["Referer"];

            if (string.IsNullOrEmpty(returnUrl))
                returnUrl = "/";

            return returnUrl;
        }
    }
}
