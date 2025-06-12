using Allup_Core.Entities;
using Allup_DataAccess.Repositories;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.SizeDtos;
using Allup_Service.Dtos.TagDtos;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Allup_Service.Service
{
    public class SizeService : ISizeService
    {
        private readonly ISizeRepository _sizeRepository;
        private readonly IMapper _mapper;
        public SizeService(ISizeRepository sizeRepository, IMapper mapper)
        {
            _sizeRepository = sizeRepository;
            _mapper = mapper;
        }


        public async Task CreateAsync(SizeCreateDto sizeCreateDto)
        {
            Size size = _mapper.Map<Size>(sizeCreateDto);
            size.CreatedAt = DateTime.UtcNow;
            size.CreatedBy = "admin"; // This should be replaced with actual user info from authentication context
            size.UpdatedAt = DateTime.UtcNow;
            size.UpdatedBy = "admin"; // This should be replaced with actual user info from authentication context
            await _sizeRepository.CreateAsync(size);
            await _sizeRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {

            var size = await _sizeRepository.GetAsync(m => m.Id == id);
            if (size == null)
            {
                throw new Exception("Tag not found");
            }

            await _sizeRepository.Delete(size);
            await _sizeRepository.SaveChangesAsync();
        }

        public async Task<SizeGetDto> DetailAsync(int id)
        {
            var size = _sizeRepository.GetAll()
                    .Where(m => m.Id == id)
                    .Select(m => _mapper.Map<SizeGetDto>(m))
                    .FirstOrDefault();

            if (size == null)
            {
                throw new Exception("Tag not found");
            }

            return size;
        }

        public async Task EditAsync(int id, SizeUpdateDto sizeUpdateDto)
        {
            var size = await _sizeRepository.GetAll().FirstOrDefaultAsync(m => m.Id == id);
            if (size == null)
            {
                throw new Exception("Tag not found");
            }

            size.Name = sizeUpdateDto.Name;
            size.UpdatedAt = DateTime.UtcNow;
            size.UpdatedBy = "admin";           // This should be replaced with actual user info from authentication context
            size.CreatedAt = DateTime.UtcNow;
            size.CreatedBy = "admin";

            _sizeRepository.Update(size);
        }

        public async Task<List<Size>> GetAllAsync()
        {
            var brand = await _sizeRepository.GetAll().ToListAsync();
            return brand.Select(m => new Size
            {
                Id = m.Id,
                Name = m.Name,
                CreatedAt = m.CreatedAt,
                CreatedBy = m.CreatedBy,
                UpdatedAt = m.UpdatedAt,
                UpdatedBy = m.UpdatedBy
            }).ToList();
        }

        public Task<SizeGetDto> GetByIdAsync(int sizeId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsExistAsync(int id)
        {
            return await _sizeRepository.IsExistAsync(p => p.Id == id);
        }
    }
}
