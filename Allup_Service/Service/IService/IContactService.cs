using Allup_Service.UI.Dtos;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Allup_Service.Service.IService
{
    public interface IContactService
    {
        Task<bool> SendEmailAsync(ContactDto dto, ModelStateDictionary ModelState);
    }
}
