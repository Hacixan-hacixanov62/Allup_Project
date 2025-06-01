using Allup_Core.Entities;
using Allup_DataAccess.Repositories;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.BannerDtos;
using Allup_Service.Dtos.CategoryDtos;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Service.Service
{
    public class BannerService : IBannerService
    {
        private readonly IBannerRepository _bannerRepository;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public BannerService(IBannerRepository bannerRepository, IMapper mapper, IWebHostEnvironment env)
        {
            _bannerRepository = bannerRepository;
            _mapper = mapper;
            _env = env;
        }


        public async Task CreateAsync(BannerCreateDto bannerCreateDto)
        {
            if (bannerCreateDto.ImageFile == null)
            {
                throw new ArgumentNullException("ImageFile", "This area is required!");
            }

            string folderPath = Path.Combine(_env.WebRootPath, "Uploads/Banners");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = bannerCreateDto.ImageFile.FileName;

            if (fileName.Length > 64)
            {
                fileName = fileName.Substring(fileName.Length - 64, 64);
            }

            fileName = Guid.NewGuid().ToString() + fileName;

            string path = Path.Combine(folderPath, fileName);

            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                await bannerCreateDto.ImageFile.CopyToAsync(fileStream);
            }

            Banner banner = _mapper.Map<Banner>(bannerCreateDto);
            banner.Image = fileName;

            await _bannerRepository.CreateAsync(banner);
            await _bannerRepository.SaveChangesAsync();
        }

        public Task DeleteAsync(int id)
        {
           var banner = _bannerRepository.GetAll().FirstOrDefault(s => s.Id == id);
            if (banner == null)
            {
                throw new ArgumentNullException("Banner", "This banner does not exist!");
            }
            string path = Path.Combine(_env.WebRootPath, "Uploads/Banners", banner.Image);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
            _bannerRepository.Delete(banner);
            return _bannerRepository.SaveChangesAsync();    
        }
         
        public async Task<BannerGetDto> DetailAsync(int id)
        {
            var banner = _bannerRepository.GetAll()
          .Where(s => s.Id == id)
          .Select(s => new BannerGetDto
          {
              RedirectUrl = s.RedirectUrl,
              ImageUrl = s.Image
          })
          .FirstOrDefault();

            if (banner == null)
            {
                throw new Exception("Banner Notfound");
            }

            return banner;
        }

        public async Task EditAsync(int id, BannerUpdateDto bannerUpdateDto)
        {
            var banner = _bannerRepository.GetAll().FirstOrDefault(c => c.Id == id);
            if (banner == null)
            {
                throw new Exception("Category not found");
            }

            string folderPath = Path.Combine(_env.WebRootPath, "Uploads/Banners");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (bannerUpdateDto.ImageFile != null)
            {
                // Köhnə şəkili sil
                string oldImagePath = Path.Combine(folderPath, banner.Image);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                // Yeni şəkilin adını formalaşdır
                string fileName = bannerUpdateDto.ImageFile.FileName;
                if (fileName.Length > 64)
                {
                    fileName = fileName.Substring(fileName.Length - 64, 64);
                }
                fileName = Guid.NewGuid().ToString() + fileName;

                string newPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await bannerUpdateDto.ImageFile.CopyToAsync(stream);
                }

                banner.Image = fileName;
            }

            //Banner updatedBanner = _mapper.Map<Banner>(bannerUpdateDto);

            banner.IsActivated = bannerUpdateDto.IsActivated;
            banner.RedirectUrl = bannerUpdateDto.RedirectUrl;

            _bannerRepository.Update(banner);
            await _bannerRepository.SaveChangesAsync();
        }

        public async Task<List<Banner>> GetAllAsync()
        {
            var banners = await _bannerRepository.GetAll().ToListAsync();

            return banners.Select(s => new Banner
            {
                Id = s.Id,
               RedirectUrl = s.RedirectUrl,
                Image = s.Image,

            }).ToList();
        }

        public Task<BannerGetDto> GetByIdAsync(int bannerId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsExistAsync(int id)
        {
            return await _bannerRepository.IsExistAsync(m=>m.Id ==id);
        }
    }
}
