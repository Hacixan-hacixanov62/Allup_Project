using Allup_Core.Entities;
using Allup_DataAccess.Repositories;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.ColorDtos;
using Allup_Service.Dtos.TagDtos;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Allup_Service.Service
{
    public class ColorService : IColorService
    {
        private readonly IColorRepository _colorRepository;
        private readonly IMapper _mapper;
        public ColorService(IColorRepository colorRepository, IMapper mapper)
        {
            _colorRepository = colorRepository;
            _mapper = mapper;
        }
        public async Task CreateAsync(ColorCreateDto sizeCreateDto)
        {
            Color tag = _mapper.Map<Color>(sizeCreateDto);
            tag.CreatedAt = DateTime.UtcNow;
            tag.CreatedBy = "admin"; // This should be replaced with actual user info from authentication context
            tag.UpdatedAt = DateTime.UtcNow;
            tag.UpdatedBy = "admin"; // This should be replaced with actual user info from authentication context
            await _colorRepository.CreateAsync(tag);
            await _colorRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var tag = await _colorRepository.GetAsync(m => m.Id == id);
            if (tag == null)
            {
                throw new Exception("Tag not found");
            }

            await _colorRepository.Delete(tag);
            await _colorRepository.SaveChangesAsync();
        }

        public async Task<ColorGetDto> DetailAsync(int id)
        {
            var tag = _colorRepository.GetAll()
                  .Where(m => m.Id == id)
                  .Select(m => _mapper.Map<ColorGetDto>(m))
                  .FirstOrDefault();

            if (tag == null)
            {
                throw new Exception("Color not found");
            }

            return tag;
        }

        public async Task EditAsync(int id, ColorUpdateDto tagUpdateDto)
        {
            var brand = await _colorRepository.GetAll().FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                throw new Exception("Color not found");
            }

            brand.Name = tagUpdateDto.Name;
            brand.UpdatedAt = DateTime.UtcNow;
            brand.UpdatedBy = "admin";           // This should be replaced with actual user info from authentication context
            brand.CreatedAt = DateTime.UtcNow;
            brand.CreatedBy = "admin";

            _colorRepository.Update(brand);
            await _colorRepository.SaveChangesAsync();
        }


        public async Task<List<Color>> GetAllAsync()
        {
            var brand = await _colorRepository.GetAll().ToListAsync();
            return brand.Select(m => new Color
            {
                Id = m.Id,
                Name = m.Name,
                CreatedAt = m.CreatedAt,
                CreatedBy = m.CreatedBy,
                UpdatedAt = m.UpdatedAt,
                UpdatedBy = m.UpdatedBy
            }).ToList();
        }

        public Task<ColorGetDto> GetByIdAsync(int sizeId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsExistAsync(int id)
        {
            return await _colorRepository.IsExistAsync(p => p.Id == id);
        }
    }
}
