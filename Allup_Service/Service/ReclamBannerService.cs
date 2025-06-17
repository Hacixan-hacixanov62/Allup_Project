using Allup_Core.Entities;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.ReclamBannerDtos;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace Allup_Service.Service
{
    public class ReclamBannerService : IReclamBannerService
    {
        private readonly IMapper _mapper;
        private readonly IReclamBannerRepository _reclamBannerRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ReclamBannerService(IReclamBannerRepository reclamBannerRepository, IMapper mapper, ICloudinaryService cloudinaryService, IHttpContextAccessor httpContextAccessor)
        {
            _reclamBannerRepository = reclamBannerRepository;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task CreateAsync(ReclamBannerCreateDto bannerCreateDto)
        {
           ReclamBanner reclamBanner = _mapper.Map<ReclamBanner>(bannerCreateDto);
            if (bannerCreateDto.ImageFile is not null)
            {
                reclamBanner.ImageUrl = await _cloudinaryService.FileCreateAsync(bannerCreateDto.ImageFile);
            }

            var username = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            reclamBanner.CreatedAt = DateTime.UtcNow;
            reclamBanner.UpdatedAt = DateTime.UtcNow;
            reclamBanner.CreatedBy = username;
            reclamBanner.UpdatedBy = username;
            reclamBanner.RedirectUrl = bannerCreateDto.RedirectUrl;
            await _reclamBannerRepository.CreateAsync(reclamBanner);
            await _reclamBannerRepository.SaveChangesAsync();

        }

        public async Task DeleteAsync(int id)
        {
            var reclamBanner =await _reclamBannerRepository.GetAll().FirstOrDefaultAsync(m => m.Id == id);

            if(reclamBanner == null)
            {
                throw new Exception("ReclamBanner not found");
            }
            
           await _reclamBannerRepository.Delete(reclamBanner);
            await _reclamBannerRepository.SaveChangesAsync();

        }

        public async Task<ReclamBannerDetailDto> DetailAsync(int id)
        {
            var reclamBanner =await _reclamBannerRepository.GetAll()
                                              .Where(m => m.Id == id)
                                              .Select(m => _mapper.Map<ReclamBannerDetailDto>(m))
                                              .FirstOrDefaultAsync();
            if (reclamBanner == null)
            {
                throw new Exception("ReclamBanner not found");
            }

            return reclamBanner;
        }

        public async Task EditAsync(int id, ReclamBannerUpdateDto bannerUpdateDto)
        {
           var reclamBanner =await _reclamBannerRepository.GetAll().FirstOrDefaultAsync(m => m.Id == id);
            if (reclamBanner == null)
            {
                throw new Exception("ReclamBanner not found");
            }
            if (bannerUpdateDto.ImageFile is not null)
            {
                reclamBanner.ImageUrl = _cloudinaryService.FileCreateAsync(bannerUpdateDto.ImageFile).Result;
            }
             _reclamBannerRepository.Update(reclamBanner);
            await _reclamBannerRepository.SaveChangesAsync();
        }

        public async Task<List<ReclamBanner>> GetAllAsync()
        {
            var reclamBanners = await _reclamBannerRepository.GetAll().ToListAsync();

            return reclamBanners.Select(m=> new ReclamBanner
            {
                Id = m.Id,
                ImageUrl = m.ImageUrl
            }).ToList();
        }

        public Task<ReclamBanner> GetByIdAsync(int bannerId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsExistAsync(int id)
        {
           return await _reclamBannerRepository.IsExistAsync(m => m.Id == id);
        }
    }
}
