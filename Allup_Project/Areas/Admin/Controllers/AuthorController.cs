using Allup_DataAccess.DAL;
using Allup_Service.Dtos.AuthorDtos;
using Allup_Service.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Allup_Project.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin,Superadmin")]
    public class AuthorController : Controller
    {
        private readonly IAuthorService _authorService;
        private readonly AppDbContext _context;
        public AuthorController(IAuthorService authorService, AppDbContext context)
        {
            _authorService = authorService;
            _context = context;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            int pageCount = (int)Math.Ceiling((decimal)_context.Authors.Count() / 10);

            if (pageCount == 0)
                pageCount = 1;

            ViewBag.PageCount = pageCount;

            if (page > pageCount)
                page = pageCount;

            if (page <= 0)
                page = 1;

            ViewBag.CurrentPage = page;

            var authors = await _context.Authors.OrderByDescending(x => x.Id).Skip((page - 1) * 10).Take(10).ToListAsync();
            return View(authors);

        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] AuthorCreateDto chefCreateDto)
        {
            if (!ModelState.IsValid)
            {
                return View(chefCreateDto);
            }
            var isExistDescription = await _context.Authors.AnyAsync(x => x.Description.ToLower() == chefCreateDto.Description.ToLower());
            var isExistBiographia = await _context.Authors.AnyAsync(x => x.Biographia.ToLower() == chefCreateDto.Biographia.ToLower());


            if (isExistDescription)
            {
                ModelState.AddModelError("Description", "Description already exist");
                return View(chefCreateDto);
            }
            if (isExistBiographia)
            {
                ModelState.AddModelError("Biographia", "Biographia already exist");
                return View(chefCreateDto);
            }

            await _authorService.CreateAsync(chefCreateDto);
            return RedirectToAction(nameof(Index));

        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chef = await _authorService.DetailAsync(id.Value);
            if (chef == null)
            {
                return NotFound();
            }

            return View(new AuthorUpdateDto
            {
                Name = chef.Name,
                Surname = chef.Surname,
                Description = chef.Description,
                Biographia = chef.Biographia,
                ImageUrl = chef.ImageUrl
            });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AuthorUpdateDto chefUpdateDto)
        {
            var existChef = await _context.Authors.FirstOrDefaultAsync(x => x.Id == id);
            if (existChef is null)
                return BadRequest();

            if (!ModelState.IsValid)
                return View(chefUpdateDto);

            var isExistBiographia = await _context.Authors.AnyAsync(x => x.Biographia.ToLower() == chefUpdateDto.Biographia.ToLower() && x.Id != id);

            if (isExistBiographia)
            {
                ModelState.AddModelError("Biographia", "Biographia already exist");
                return View(chefUpdateDto);
            }

            await _authorService.EditAsync(id, chefUpdateDto);
            return RedirectToAction("Index");

        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _authorService.DeleteAsync(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("admin/author/detail")]
        public async Task<IActionResult> Detail(int id)
        {
            try
            {
                var slider = await _authorService.DetailAsync(id);
                return View(slider);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message); // Slider tapılmadıqda
            }
        }



    }
}
