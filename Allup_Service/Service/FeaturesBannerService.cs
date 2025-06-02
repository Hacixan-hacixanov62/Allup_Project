using Allup_Core.Entities;
using Allup_DataAccess.Repositories;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.FeaturesBannerDtos;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;


namespace Allup_Service.Service
{
    public class FeaturesBannerService : IFeaturesBannerService
    {
        private readonly IFeaturesBannerRepository _featuresBannerRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;

        public FeaturesBannerService(IFeaturesBannerRepository featuresBannerRepository, IWebHostEnvironment env, IMapper mapper)
        {
            _featuresBannerRepository = featuresBannerRepository;
            _env = env;
            _mapper = mapper;
        }

        public async Task CreateAsync(FeaturesBannerCreateDto bannerCreateDto)
        {
            if (bannerCreateDto.ImageFile == null)
            {
                throw new ArgumentNullException("ImageFile", "This area is required!");
            }

            string folderPath = Path.Combine(_env.WebRootPath, "Uploads/FeaturesBanners");

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

            FeaturesBanner featuresBanner = _mapper.Map<FeaturesBanner>(bannerCreateDto);
            featuresBanner.ImageUrl = fileName;

            await _featuresBannerRepository.CreateAsync(featuresBanner);
            await _featuresBannerRepository.SaveChangesAsync();

        }

        public async Task DeleteAsync(int id)
        {
            var banner = _featuresBannerRepository.GetAll().FirstOrDefault(s => s.Id == id);
            if(banner == null)
            {
                throw new ArgumentNullException("Banner", "Banner not found");
            }

            string path = Path.Combine(_env.WebRootPath, "Uploads/FeaturesBanners", banner.ImageUrl);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

           await _featuresBannerRepository.Delete(banner);
            await _featuresBannerRepository.SaveChangesAsync();
        }

        public async Task<FeaturesBannerDetailDto> DetailAsync(int id)
        {
            var banner =_featuresBannerRepository.GetAll()
                                    .Where(b => b.Id == id)
                                    .Select(b => _mapper.Map<FeaturesBannerDetailDto>(b))
                                    .FirstOrDefault();
            if(banner ==null)
            {
                throw new ArgumentNullException("Banner", "This banner does not exist!");
            }

            return banner;
        }

        public async Task EditAsync(int id, FeaturesBannerUpdateDto bannerUpdateDto)
        {
            var banner = _featuresBannerRepository.GetAll().FirstOrDefault(c => c.Id == id);
            if (banner == null)
            {
                throw new Exception("Category not found");
            }

            string folderPath = Path.Combine(_env.WebRootPath, "Uploads/FeaturesBanners");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (bannerUpdateDto.ImageFile != null)
            {
                // Köhnə şəkili sil
                string oldImagePath = Path.Combine(folderPath, banner.ImageUrl);
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

                banner.ImageUrl = fileName;
            }

            banner.Title = bannerUpdateDto.Title;
            banner.Desc = bannerUpdateDto.Desc;

            _featuresBannerRepository.Update(banner);
            await _featuresBannerRepository.SaveChangesAsync();
        }

        public async Task<List<FeaturesBanner>> GetAllAsync()
        {
            var featureBanner = await _featuresBannerRepository.GetAll().ToListAsync();

            return featureBanner.Select(m=> new FeaturesBanner
            {
                Id =m.Id,
                Desc = m.Desc,
                ImageUrl = m.ImageUrl,
                Title =m.Title
            }).ToList();
        }

        public Task<FeaturesBanner> GetByIdAsync(int bannerId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsExistAsync(int id)
        {
            return await _featuresBannerRepository.IsExistAsync(m => m.Id == id);
        }
    }
}
