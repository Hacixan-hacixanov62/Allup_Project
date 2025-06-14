using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_Project.ViewModels.AuthVm;
using Allup_Service.Service;
using Allup_Service.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;

namespace Allup_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, RoleManager<IdentityRole> roleManager, AppDbContext context, IAuthService authService, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
            _authService = authService;
            _emailService = emailService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserRegisterVM userRegisterVm)
        {
            if (!ModelState.IsValid)
            {
                return View(userRegisterVm);
            }
            AppUser appUser = await _userManager.FindByNameAsync(userRegisterVm.UserName);
            if (appUser != null)
            {
                ModelState.AddModelError("UserName", "This username already exists");
                return View();
            }
            appUser = new()
            {
                UserName = userRegisterVm.UserName,
                FullName = userRegisterVm.FullName,
                Email = userRegisterVm.Email
            };
            var result = await _userManager.CreateAsync(appUser, userRegisterVm.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    //if (error.Code == "DuplicateEmail")
                    //{
                    //    ModelState.AddModelError("Email", "The email already exists");

                    //}
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }

            await _userManager.AddToRoleAsync(appUser, "Member");
            var token = await _userManager.GeneratePasswordResetTokenAsync(appUser);
            var url = Url.Action("Login", "Account", new { email = appUser.Email, token = token }, Request.Scheme);


            //create email
            using StreamReader reader = new StreamReader("wwwroot/templete/emailConfirm.html");
            var body = reader.ReadToEnd();
            body = body.Replace("{{url}}", url);
            body = body.Replace("{{username}}", appUser.UserName);
            await _emailService.SendEmailAsync(new() { Body = body, Subject = "Confirm Email", ToEmail = appUser.Email });//yoxla
            TempData["Success"] = "Email sended to" + appUser.Email;

            return RedirectToAction("Login");
        }


        public IActionResult Login()
        {
            return View(); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login( UserLoginVm userLoginVm, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(userLoginVm);
            }

            AppUser appUser = await _userManager.FindByNameAsync(userLoginVm.UserNameOrEmail);
            if (appUser == null || !await _userManager.IsInRoleAsync(appUser, "Member")) //|| 
            {

                appUser = await _userManager.FindByEmailAsync(userLoginVm.UserNameOrEmail);
                if (appUser == null)
                {
                    ModelState.AddModelError("", "UserName is inValid or Email ...");
                    return View();
                }
            }

            var result = await _signInManager.PasswordSignInAsync(appUser, userLoginVm.Password, userLoginVm.RememberMe, true);
            //if (!appUser.EmailConfirmed)
            //{
            //    ModelState.AddModelError("", "Invalid Email or Username ...");         ////Emaili confirm etmeden login ola bilmirsen Amma mende confimrde problem var
            //    return View(userLoginVm);
            //}
            if (result.IsLockedOut)
            {
                ModelState.AddModelError("", "Account is lockOut ...");
                return View(userLoginVm);
            }

            if (!result.Succeeded)
            {
           
                ModelState.AddModelError("", "Invalid username or email ...");
                return View(userLoginVm);

            }
            HttpContext.Response.Cookies.Append("RestaurantCart", "");

            return returnUrl != null ? Redirect(returnUrl) : RedirectToAction("Index", "Home");
        }


        [Authorize(Roles ="Member")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public async Task<IActionResult> Profile(string tab = "Dashboard")
        {
            ViewBag.Tab = tab;


            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("login");
            }
            UserProfileVM userProfileVm = new UserProfileVM();
            userProfileVm.UserProfileUpdateVM = new()
            {
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
            };
            //userProfileVm.Orders = _context.Orders
            //    .Include(m => m.User)
            //    .Where(m => m.UserId == user.Id).ToList();

            return View(userProfileVm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Member")]
        public async Task<IActionResult> Profile(UserProfileUpdateVM userProfileUpdateVM, string tab = "Profile")
        {
            ViewBag.Tab = tab;

            UserProfileVM userProfileVm = new();
            userProfileVm.UserProfileUpdateVM = userProfileUpdateVM;

            if (!ModelState.IsValid)
            {
                return View(userProfileVm);                                // Account hissesine bax ve orada balaca problem var onu hell et
            }
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {   
                return RedirectToAction("login");
            }

            user.FullName = userProfileUpdateVM.FullName;
            user.Email = userProfileUpdateVM.Email;
            user.UserName = userProfileUpdateVM.UserName;

            if (userProfileUpdateVM.NewPassword != null)
            {
                if (userProfileUpdateVM.CurrentPassword == null)
                {
                    ModelState.AddModelError("CurrentPassword", "Current password is required");
                    return View(userProfileVm);
                }
                var response = await _userManager.ChangePasswordAsync(user, userProfileUpdateVM.CurrentPassword, userProfileUpdateVM.NewPassword);
                if (!response.Succeeded)
                {
                    foreach (var error in response.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(userProfileVm);
                }
            }
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(userProfileVm);
            }
            await _signInManager.SignInAsync(user, true);
            return RedirectToAction("Index", "Home");



        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVm forgotPasswordVm)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(forgotPasswordVm.Email);
            if (user == null)//|| !await _userManager.IsInRoleAsync(user, "Member")
            {
                return RedirectToAction("notfound", "error");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var url = Url.Action("ResetPassword", "Account", new { email = user.Email, token = token }, Request.Scheme);


            //create email
            using StreamReader reader = new StreamReader("wwwroot/templete/resetpassword.html");
            var body = reader.ReadToEnd();
            body = body.Replace("{{url}}", url);
            body = body.Replace("{{username}}", user.UserName);
            //  _emailService.SendEmail(user.Email, "ResetPassword", body);
            await _emailService.SendEmailAsync(new() { Body = body, Subject = "ResetPassword", ToEmail = user.Email });//yoxla
            TempData["Success"] = "Email sended to" + user.Email;
            return View();

        }

        public async Task<IActionResult> VerifyEmail(string email, string token)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.IsInRoleAsync(user, "Member"))
            {
                return RedirectToAction("notfound", "error");
            }
            await _userManager.ConfirmEmailAsync(user, token);

            return RedirectToAction("Login");
        }


        public async Task<IActionResult> VerifyPassword(string token, string email)
        {
            TempData["token"] = token;
            TempData["email"] = email;
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return RedirectToAction("notfound", "error");
            }
            if (!(await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", token)))
            {
                return RedirectToAction("notfound", "error");
            }

            return RedirectToAction("ResetPassword");
        }

        public IActionResult ResetPassword(string token, string email)
        {
            if (token == null || email == null) return BadRequest();

            var model = new ResetPasswordVM { Email = email, Token = token };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM resetPasswordVM)
        {

            TempData["token"] = resetPasswordVM.Token;
            TempData["email"] = resetPasswordVM.Email;

            if (!ModelState.IsValid)
            {
                return View();
            }
            var user = await _userManager.FindByEmailAsync(resetPasswordVM.Email);
            if (user == null || !await _userManager.IsInRoleAsync(user, "Member"))
            {
                return View();
            }
            if (!await _userManager.VerifyUserTokenAsync(user, _userManager.Options.Tokens.PasswordResetTokenProvider, "ResetPassword", resetPasswordVM.Token))
            {
                return RedirectToAction("notfound", "error");
            }

            var result = await _userManager.ResetPasswordAsync(user, resetPasswordVM.Token, resetPasswordVM.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View();
            }


            return RedirectToAction("Login");
        }



    }
}
