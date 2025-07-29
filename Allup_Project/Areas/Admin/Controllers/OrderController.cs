using Allup_Service.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace Allup_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Superadmin")]

    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;   
        }
        [OutputCache(Duration = 60, Tags = new[] { "Tag" })]
        public async Task<IActionResult> Index(int page = 1, int take = 10)
        {
            var result = await _orderService.GetPaginatedAsync(page, take);

            ViewBag.PageCount = result.PageCount;
            ViewBag.CurrentPage = result.CurrentPage;

            return View(result.Orders);
        }

        [OutputCache(Duration = 60, Tags = new[] { "Tag" })]
        public async Task<IActionResult> Detail(int id)
        {
            await _orderService.DetailAsync(id);
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Next(int id)
        {
            await _orderService.NextOrderStatusAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Prev(int id)
        {
            await _orderService.PrevOrderStatusAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CancelOrder(int id)
        {
            await _orderService.CancelOrderAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Repair(int id)
        {

            await _orderService.RepairOrderAsync(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Cancel(int id)
        {
            await _orderService.CancelOrderAsync(id);
            return RedirectToAction("Index");
        }
    }
}
