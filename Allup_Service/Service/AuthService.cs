

using Allup_Core.Entities;
using Allup_Service.Dtos.AuthDtos;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Allup_Service.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IMapper _mapper;
        public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IHttpContextAccessor contextAccessor, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _contextAccessor = contextAccessor;
            _mapper = mapper;
        }


        public Task<bool> ChangeUserRoleAsync(UserChangeRoleDto dto)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CheckResetPasswordTokenAsync(ResetPasswordDto dto) 
        {
            throw new NotImplementedException();
        }

        public Task<List<UserGetDto>> GetAllUserAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserGetDto> GetUserAsync(string id)
        {
            throw new NotImplementedException();
        }

        public  Task<bool> LoginAsync(LoginDto dto, ModelStateDictionary ModelState)
        {
            throw new NotImplementedException();

        }

        public Task<bool> LogoutAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> RegisterAsync(RegisterDto dto, ModelStateDictionary ModelState)
        {
            throw new NotImplementedException();
        }

        public Task RemoveBotsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<bool> ResetPasswordAsync(ResetPasswordDto dto, ModelStateDictionary ModelState)
        {
            throw new NotImplementedException();
        }

        public Task<bool> SendForgotPasswordEmailAsync(ForgotPasswordDto dto, ModelStateDictionary ModelState)
        {
            throw new NotImplementedException();
        }

        public Task<bool> VerifyEmailAsync(VerifyEmailDto dto, ModelStateDictionary ModelState)
        {
            throw new NotImplementedException();
        }
    }
}
