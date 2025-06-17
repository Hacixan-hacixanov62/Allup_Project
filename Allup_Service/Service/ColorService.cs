using Allup_Core.Entities;
using Allup_DataAccess.Repositories;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.ColorDtos;
using Allup_Service.Dtos.TagDtos;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Allup_Service.Service
{
    public class ColorService : IColorService
    {
        private readonly IColorRepository _colorRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ColorService(IColorRepository colorRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor = null)
        {
            _colorRepository = colorRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task CreateAsync(ColorCreateDto sizeCreateDto)
        {
            var usernsme = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            Color tag = _mapper.Map<Color>(sizeCreateDto);
            tag.CreatedAt = DateTime.UtcNow;
            tag.CreatedBy = usernsme;
            tag.UpdatedAt = DateTime.UtcNow;
            tag.UpdatedBy = usernsme;
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
            var usernsme = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            var brand = await _colorRepository.GetAll().FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                throw new Exception("Color not found");
            }

            brand.Name = tagUpdateDto.Name;
            brand.UpdatedAt = DateTime.UtcNow;
            brand.UpdatedBy = usernsme;
            brand.CreatedAt = DateTime.UtcNow;
            brand.CreatedBy = usernsme;

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
