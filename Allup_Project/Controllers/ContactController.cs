using Allup_Service.Service.IService;
using Allup_Service.UI.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Allup_Project.Controllers
{
    public class ContactController : Controller
    {
        private readonly IContactService _contactService;
        public ContactController(IContactService contactService)
        {
            _contactService = contactService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(ContactDto dto)
        {
            var result = await _contactService.SendEmailAsync(dto, ModelState);

            if (!result)
            {
                return View(dto);
            }

            return RedirectToAction(nameof(Index));

        }
    }
}
