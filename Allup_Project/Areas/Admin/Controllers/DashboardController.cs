using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Allup_Project.Areas.Admin.Controllers
{
    
    [Area("Admin")]
    [Authorize(Roles = "Admin,Superadmin")]
    [OutputCache(Duration = 60, Tags = new[] { "Tag" })]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
