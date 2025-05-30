using Microsoft.AspNetCore.Mvc;

namespace Allup_Project.Areas.Admin.Controllers
{
    [Area("Admin")] 
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
