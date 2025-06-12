using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Helpers;
using Allup_Service.Dtos.BannerDtos;
using Allup_Service.Dtos.CategoryDtos;
using Allup_Service.Service;
using Allup_Service.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Superadmin")]

    public class BannerController : Controller
    {
        private readonly IBannerService _bannerService;
        private readonly AppDbContext _context;

        public BannerController(IBannerService bannerService, AppDbContext context)
        {
            _bannerService = bannerService;
            _context = context;
        }

        public async Task<IActionResult> Index(int page=1,int take=4)
        {
            var banners = await _bannerService.GetAllAsync();
          //  PaginatedList<Banner> paginateList =PaginatedList<Banner>.Create(banners, page, take);
            return View(banners);
        }

        public IActionResult  Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BannerCreateDto bannerCreateDto)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(bannerCreateDto);
            //}

            await _bannerService.CreateAsync(bannerCreateDto);

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var banner = await _bannerService.DetailAsync(id.Value);
            if (banner == null)
            {
                return NotFound();
            }

            var bannrUpdatedto = new BannerUpdateDto
            {
               ImageUrl = banner.ImageUrl,
               RedirectUrl=banner.RedirectUrl,
            };


            return View(bannrUpdatedto);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,BannerUpdateDto bannerUpdateDto)
        {
            

            try
            {
                await _bannerService.EditAsync(id, bannerUpdateDto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(bannerUpdateDto);
            }
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
           await _bannerService.DeleteAsync(id);
            return RedirectToAction("Index");
        }


        [HttpGet("admin/banner/detail")]
        public async Task<IActionResult> Detail(int id)
        {
            var banner = await _bannerService.DetailAsync(id);
            if (banner == null)
                return NotFound();
            return View(banner);
        }
    }
}
