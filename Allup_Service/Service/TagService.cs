
using Allup_Core.Entities;
using Allup_DataAccess.Repositories;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.BrandDtos;
using Allup_Service.Dtos.TagDtos;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Allup_Service.Service
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public TagService(ITagRepository tagRepository, IMapper mapper, IHttpContextAccessor httpContextAccessor = null)
        {
            _tagRepository = tagRepository;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }


        public async Task CreateAsync(TagCreateDto tagCreateDto)
        {
            var usernsme = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            Tag tag = _mapper.Map<Tag>(tagCreateDto);
            tag.CreatedAt = DateTime.UtcNow;
            tag.CreatedBy = usernsme;
            tag.UpdatedAt = DateTime.UtcNow;
            tag.UpdatedBy = usernsme;
            await _tagRepository.CreateAsync(tag);
            await _tagRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var tag =await _tagRepository.GetAsync(m => m.Id == id);
            if(tag == null)
            {
                throw new Exception("Tag not found");
            }

             await _tagRepository.Delete(tag);
            await _tagRepository.SaveChangesAsync();
        }

        public async Task<TagGetDto> DetailAsync(int id)
        {
            var tag = _tagRepository.GetAll()
                  .Where(m=>m.Id == id)
                  .Select(m=> _mapper.Map<TagGetDto>(m))
                  .FirstOrDefault();    

            if (tag == null)
            {
                throw new Exception("Tag not found");
            }

            return tag; 
        }

        public async Task EditAsync(int id, TagUpdateDto tagUpdateDto)
        {
            var usernsme = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            var brand = await _tagRepository.GetAll().FirstOrDefaultAsync(m => m.Id == id);
            if (brand == null)
            {
                throw new Exception("Tag not found");
            }

            brand.Name = tagUpdateDto.Name;
            brand.UpdatedAt = DateTime.UtcNow;
            brand.UpdatedBy =usernsme;        
            brand.CreatedAt = DateTime.UtcNow;
            brand.CreatedBy = usernsme;

            _tagRepository.Update(brand);
            await _tagRepository.SaveChangesAsync();
        }

        public async Task<List<Tag>> GetAllAsync()
        {
            var brand = await _tagRepository.GetAll().ToListAsync();
            return brand.Select(m => new Tag
            {
                Id = m.Id,
                Name = m.Name,
                CreatedAt = m.CreatedAt,
                CreatedBy = m.CreatedBy,
                UpdatedAt = m.UpdatedAt,
                UpdatedBy = m.UpdatedBy
            }).ToList();
        }

        public Task<TagGetDto> GetByIdAsync(int tagId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsExistAsync(int id)
        {
            return await _tagRepository.IsExistAsync(p => p.Id == id);
        }
    }
}
