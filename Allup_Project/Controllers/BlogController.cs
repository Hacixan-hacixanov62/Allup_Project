using Allup_Service.Service.IService;
using Microsoft.AspNetCore.Mvc;

namespace Allup_Project.Controllers
{
    public class BlogController : Controller
    {
        private readonly IBlogService _blogService;

        public BlogController(IBlogService blogService)
        {
            _blogService = blogService;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
