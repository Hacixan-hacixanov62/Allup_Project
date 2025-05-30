using Allup_Core.Entities;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.SliderDtos;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IWebHostEnvironment _env;
        public SliderService(ISliderRepository sliderRepository, IMapper mapper, ICloudinaryService cloudinaryService, IWebHostEnvironment webHostEnvironment)
        {
            _sliderRepository = sliderRepository;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
            _env = webHostEnvironment;
        }

        public async Task CreateAsync(SliderCreateDto sliderCreateDto)
        {
            if (sliderCreateDto.NewImage == null)
                throw new ArgumentNullException("ImageFile", "This area is required!");

            string folderPath = Path.Combine(_env.WebRootPath, "Uploads/Sliders");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            string fileName = sliderCreateDto.NewImage.FileName;

            if (fileName.Length > 64)
            {
                fileName = fileName.Substring(fileName.Length - 64, 64);
            }

            fileName = Guid.NewGuid().ToString() + fileName;

            string path = Path.Combine(folderPath, fileName);

            using (FileStream fileStream = new FileStream(path, FileMode.Create))
            {
                await sliderCreateDto.NewImage.CopyToAsync(fileStream);
            }


            Slider slider = new Slider
            {
                Title = sliderCreateDto.Title,
                Desc = sliderCreateDto.Desc,
                Image = fileName
            };

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

            string folderPath = Path.Combine(_env.WebRootPath, "Uploads/Sliders");

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            if (sliderUpdateDto.NewImage != null)
            {
                // Köhnə şəkili sil
                string oldImagePath = Path.Combine(folderPath, slider.Image);
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }

                // Yeni şəkilin adını formalaşdır
                string fileName = sliderUpdateDto.NewImage.FileName;
                if (fileName.Length > 64)
                {
                    fileName = fileName.Substring(fileName.Length - 64, 64);
                }
                fileName = Guid.NewGuid().ToString() + fileName;

                string newPath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(newPath, FileMode.Create))
                {
                    await sliderUpdateDto.NewImage.CopyToAsync(stream);
                }

                slider.Image = fileName;
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
