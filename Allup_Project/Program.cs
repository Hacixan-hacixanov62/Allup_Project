using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Interceptors;
using Allup_Project.Extensions;
using Allup_Service;
using Allup_Service.Hubs;
using Allup_Service.Profiles;
using Allup_Service.Profiles.BlogCommentProfile;
using Allup_Service.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;
using System.Reflection;

namespace Allup_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;

          //  builder.Services.AddControllersWithViews()
          //      .AddNewtonsoftJson(options =>
          //options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
          //);


            builder.Services.AddBusinessServices();
            //  builder.Services.AddBussniessRepository(builder.Configuration);
          //  builder.Services.AddDalServices(builder.Configuration);
            //builder.Services.AddBusinessServices();


            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddSignalR();

            builder.Services.AddDbContext<AppDbContext>(options =>
                   options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddHttpContextAccessor();

            builder.Services.AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = true;
                opt.Password.RequiredLength = 6;
                opt.User.RequireUniqueEmail = true;

                // opt.SignIn.RequireConfirmedEmail = true; // eger emaili confirm etmemisse login ola bilmez

                opt.Lockout.MaxFailedAccessAttempts = 3; // cehd sohbeti Remmember !!!
                opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                opt.Lockout.AllowedForNewUsers = true;


            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("StripeSettings"));

            StripeConfiguration.ApiKey = builder.Configuration["StripeSettings:SecretKey"];

            builder.Services.AddScoped<BaseEntityInterceptor>();

            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

            builder.Services.AddAutoMapper(opt =>
            {
                opt.AddProfile(new MapperProfiles()); // bu usul bize eger bos constructor yoxdursa onda lazim olacaq

            });

            // Cachei silmek ucundur
            builder.Services.AddOutputCache();
            builder.Services.AddScoped<CacheClearService>();

            builder.Services.AddAutoMapper(typeof(BlogCommetProfile).Assembly);

            //builder.Services.ConfigureApplicationCookie(opt =>
            //{
            //    opt.Events.OnRedirectToLogin = opt.Events.OnRedirectToAccessDenied = context =>
            //    {
            //        var uri = new Uri(context.RedirectUri);
            //        if (context.Request.Path.Value.ToLower().StartsWith("/admin"))
            //        {
            //            context.Response.Redirect("/admin/account/login" + uri.Query);
            //        }
            //        else
            //        {
            //            context.Response.Redirect("/account/login" + uri.Query);
            //        }
            //        return Task.CompletedTask;
            //    };
            //});


            var app = builder.Build();

            app.UseMiddleware<GlobalExceptionHandler>();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.MapHub<ChatHub>("/chatHub");
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); // bu yeri yazmasaq Identity-nin login ve registeri islemeyecek , User sistemnen baglidir
            app.UseAuthorization(); // bu ise rol ve permissionler ucundur

            app.UseOutputCache(); // Cache-i istifade etmek ucundur

            app.MapControllerRoute(
               name: "areas",
               pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
