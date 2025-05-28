using Allup_Core.Entities;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.SliderDtos;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using System.Globalization;


namespace Allup_Service.Service
{
    public class SliderService : ISliderService
    {   
        private readonly ISliderRepository _sliderRepository;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IFileService _fileService;
        public SliderService(ISliderRepository sliderRepository, IMapper mapper, ICloudinaryService cloudinaryService, IFileService fileService)
        {
            _sliderRepository = sliderRepository;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
            _fileService = fileService;
        }

        public async Task CreateAsync(SliderCreateDto sliderCreateDto)
        {
            if(sliderCreateDto.ImageUrl is null)
            {
                throw new ArgumentNullException("ImageFile", "This area is required!");
            }

            Slider slider = _mapper.Map<Slider>(sliderCreateDto);
            slider.Image =await _cloudinaryService.FileCreateAsync(sliderCreateDto.Photo);

            await _sliderRepository.CreateAsync(slider);
            await _sliderRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var slider = _sliderRepository.GetAll().FirstOrDefault(s => s.Id == id);
            if (slider == null)
            {
                throw new Exception("Slider not found");
            }

            await _cloudinaryService.FileDeleteAsync(slider.Image);
            await _sliderRepository.Delete(slider); 
            await _sliderRepository.SaveChangesAsync();
        }

        public async Task<Slider> DetailAsync(int id)
        {
            var slider = _sliderRepository.GetAll()
                .Where(s=>s.Id == id)
                .Select(s => new Slider 
                {
                    Id=s.Id,
                    Desc = s.Desc,
                    Image = s.Image,
                    Title = s.Title


                }).FirstOrDefault();

            if (slider == null)
            {
                throw new Exception("Slider tapılmadı");
            }

            return slider;
        }

        public async Task EditAsync(int id, SliderUpdateDto sliderUpdateDto)
        {
          
            var slider = _sliderRepository.GetAll().FirstOrDefault(s => s.Id == id);
            if (slider == null)
                throw new Exception("Slider not found");

            
            if (sliderUpdateDto.Photo != null)
            {

                _fileService.Delete(slider.Image, "asset/images/home");

                string newImage = await _fileService.UploadFilesAsync(sliderUpdateDto.Photo, "asset/images/home");

                
                slider.Image = newImage;
            }

           
            slider.Title = sliderUpdateDto.Title;
            slider.Desc = sliderUpdateDto.Desc;
           


             _sliderRepository.Update(slider);
            await _sliderRepository.SaveChangesAsync();
        }


        public async Task<List<Slider>> GetAllAsync()
        {
            var sliders = await _sliderRepository.GetAll().ToListAsync();
            return sliders.Select(s => new Slider
            {
                Id = s.Id,
                Title = s.Title,
                Desc = s.Desc,
                Image = s.Image,
                // CreatedDate = s.CreatedDate.ToString("yyyy-MM-dd")
            }).ToList();
        }
    }
}
