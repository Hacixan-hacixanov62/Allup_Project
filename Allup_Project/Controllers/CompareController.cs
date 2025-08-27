using Allup_Service.Dtos.CompareDtos;
using Allup_Service.Dtos.WisListDtos;
using Allup_Service.Service;
using Allup_Service.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Allup_Project.Controllers
{
    public class CompareController : Controller
    {
        private readonly ICompareService _compareService;

        public CompareController(ICompareService compareService)
        {
            _compareService = compareService;
        }

        public async Task<IActionResult> Index()
        {
           var compareDetails = await _compareService.GetCompareAsync();
            return View(compareDetails);
        }

        [HttpPost]
        public async Task<IActionResult> AddCompare(int id, int count)
        {
           await _compareService.AddToCompareAsync(id,count);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCompare(int id)
        {
            var success = await _compareService.RemoveFromCompareAsync(id);                                                                                                                      
            return Json(new { success });
        }

        public async Task<IActionResult> GetCountCompare()
        {
            List<CompareDto> wishlist = new();
            if (Request.Cookies[CompareService.COMPARE_KEY] != null)
            {
                wishlist = JsonConvert.DeserializeObject<List<CompareDto>>(Request.Cookies[CompareService.COMPARE_KEY]);
            }
            var count = wishlist.Count;
            return Json(new { success = true, count });
        }

        public async Task<IActionResult> getCountCompare()
        {
            var temCount = await _compareService.GetIntAsync();

            return Json(new
            {
                count = temCount
            });
        }


    }
}
