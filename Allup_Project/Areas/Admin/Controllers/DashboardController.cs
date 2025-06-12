using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Allup_Project.Areas.Admin.Controllers
{
    
    [Area("Admin")]
    [Authorize(Roles = "Admin,Superadmin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
