using Allup_Core.Entities;
using Allup_DataAccess.Repositories;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.AboutDtos;
using Allup_Service.Exceptions;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Allup_Service.Service
{
    public class AboutService : IAboutService
    {
        private readonly IAboutRepository _aboutRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;

        public AboutService(IMapper mapper, ICloudinaryService cloudinaryService , IAboutRepository aboutRepository )
        {
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
            _aboutRepository = aboutRepository;
        }

        public async Task CreateAsync(AboutCreateDto aboutCreateDto)
        {
            About about = _mapper.Map<About>(aboutCreateDto);
            about.ImageUrl = _cloudinaryService.FileCreateAsync(aboutCreateDto.ImageFile).Result;
            await _aboutRepository.CreateAsync(about);
            await _aboutRepository.SaveChangesAsync();

        }

        public async Task DeleteAsync(int id)
        {
            var about = await _aboutRepository.GetAsync(id);
            if (about == null)
            {
                throw new NotFoundException("About not found");
            }
            if (!string.IsNullOrEmpty(about.ImageUrl))
            {
                await _cloudinaryService.FileDeleteAsync(about.ImageUrl);
            }
            await _aboutRepository.Delete(about);
            await _aboutRepository.SaveChangesAsync();
        }

        public async Task<AboutGetDto> DetailAsync(int id)
        {
            var about = await _aboutRepository.GetAsync(id);
            if (about == null)
            {
                throw new NotFoundException("About not found");
            }
            return _mapper.Map<AboutGetDto>(about);
        }

        public async Task EditAsync(int id, AboutUpdateDto aboutUpdateDto)
        {
            var about = await _aboutRepository.GetAsync(id);
            if (about == null)
            {
                throw new NotFoundException("About not found");
            }
            if (aboutUpdateDto.ImageFile != null)
            {
                if (!string.IsNullOrEmpty(about.ImageUrl))
                {
                    await _cloudinaryService.FileDeleteAsync(about.ImageUrl);
                }
                about.ImageUrl = await _cloudinaryService.FileCreateAsync(aboutUpdateDto.ImageFile);
            }
            about.Title = aboutUpdateDto.Title;
            about.Desc = aboutUpdateDto.Desc;
            about.Desc1 = aboutUpdateDto.Desc1;
            about.Desc2 = aboutUpdateDto.Desc2;
            about.Desc3 = aboutUpdateDto.Desc3;
            about.Title1 = aboutUpdateDto.Title1;
            about.Title2 = aboutUpdateDto.Title2;
            about.Title3 = aboutUpdateDto.Title3;

               _aboutRepository.Update(about);
            await _aboutRepository.SaveChangesAsync();
        }

        public async Task<List<About>> GetAllAsync()
        {
            var abouts = await _aboutRepository.GetAll().ToListAsync();

            return abouts.Select(a=> new About
            {
                Id = a.Id,
                Title = a.Title,
                Title1 = a.Title1,
                Title2 = a.Title2,
                Title3 = a.Title3,
                Desc = a.Desc,
                Desc1 = a.Desc1,
                Desc2 = a.Desc2,
                Desc3 = a.Desc3,
                ImageUrl = a.ImageUrl

            }).ToList();
        }

        public async Task<AboutGetDto> GetByIdAsync(int aboutId)
        {
            var about = await _aboutRepository.GetAsync(aboutId);
            if (about == null)
            {
                throw new NotFoundException("About not found");
            }
            return _mapper.Map<AboutGetDto>(about);
        }

        public  Task<bool> IsExistAsync(int id)
        {
            return _aboutRepository.IsExistAsync(m => m.Id == id);

        }
    }
}
