using Microsoft.AspNetCore.Mvc;

namespace Allup_Project.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
