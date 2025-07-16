using Allup_Core.Entities;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.AuthorDtos;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Allup_Service.Service
{
    public class AuthorService : IAuthorService
    {
        private readonly IAuthorRepository _authorRepository;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthorService(IHttpContextAccessor httpContextAccessor, IMapper mapper, IAuthorRepository authorRepository, ICloudinaryService cloudinaryService)
        {
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _authorRepository = authorRepository;
            _cloudinaryService = cloudinaryService;
        }


        public async Task CreateAsync(AuthorCreateDto chefCreateDto)
        {
            Author author =_mapper.Map<Author>(chefCreateDto);

            var usernsme = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value;
            author.ImageUrl = await _cloudinaryService.FileCreateAsync(chefCreateDto.Image);
            author.CreatedBy = usernsme;
            author.UpdatedBy = usernsme;
            author.CreatedAt = DateTime.UtcNow;
            author.UpdatedAt = DateTime.UtcNow;
            await _authorRepository.CreateAsync(author);
            await _authorRepository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var product = _authorRepository.GetAll().FirstOrDefault(s => s.Id == id);
            if (product == null)
            {
                throw new Exception("Author tapılmadı");
            }

            await _authorRepository.Delete(product);
            await _authorRepository.SaveChangesAsync();
        }

        public async Task<Author> DetailAsync(int id)
        {
            var chef = await _authorRepository.GetAll()
                 .Where(s => s.Id == id)
                 .Select(s => new Author
                 {
                     Id = s.Id,
                     Name = s.Name,
                     Surname = s.Surname,
                     Description = s.Description,
                     Biographia = s.Biographia,
                     ImageUrl = s.ImageUrl
                 })
                 .FirstOrDefaultAsync();

            if (chef == null)
            {
                throw new Exception("Author tapılmadı");
            }

            return chef;
        }

        public async Task EditAsync(int id, AuthorUpdateDto chefUpdateDto)
        {
            var chef = _authorRepository.GetAll().FirstOrDefault(s => s.Id == id);
            if (chef == null)
            {
                throw new Exception("Author tapılmadı");
            }

            AuthorUpdateDto dto = _mapper.Map<AuthorUpdateDto>(chef);

            chef = _mapper.Map(chefUpdateDto, chef);
            if (chefUpdateDto is not null)
            {
                chef.ImageUrl = await _cloudinaryService.FileCreateAsync(chefUpdateDto.Image);
            }

            _authorRepository.Update(chef);
            await _authorRepository.SaveChangesAsync();
        }

        public async Task<List<Author>> GetAllAsync()
        {
            var chefs = await _authorRepository.GetAll().ToListAsync();
            return chefs.Select(s => new Author
            {
                Id = s.Id,
                Name = s.Name,
                Surname = s.Surname,
                Description = s.Description,
                Biographia = s.Biographia,
                ImageUrl = s.ImageUrl

            }).ToList();
        }
    }
}
