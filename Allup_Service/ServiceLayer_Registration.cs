using Allup_DataAccess.Repositories;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Profiles;
using Allup_Service.Service;
using Allup_Service.Service.IService;
using Allup_Service.UI.Services;
using Allup_Service.UI.Services.IServices;
using Allup_Service.Validators.AppUserValidators;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;


namespace Allup_Service
{
    public static class ServiceLayer_Registration
    {
        public static void AddBusinessServices(this IServiceCollection services)
        {
            var builder = WebApplication.CreateBuilder();
            var config = builder.Configuration;

            AddServices(services);

            builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

            builder.Services.AddAutoMapper(opt =>
            {
                opt.AddProfile(new MapperProfiles()); // bu usul bize eger bos constructor yoxdursa onda lazim olacaq
            });

            services.AddHttpClient();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddHttpContextAccessor();

            services.AddFluentValidationAutoValidation();
            services.AddValidatorsFromAssemblyContaining(typeof(LoginDtoValidators));


        } 
        
        private static void AddServices(IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<ILayoutService, LayoutService>();

            services.AddScoped<ISliderService, SliderService>();
            services.AddScoped<ISliderRepository, SliderRepository>();

            services.AddScoped<ICloudinaryService, CloudinaryService>();

            services.AddScoped<IFileService, FileService>();

            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductRepository, ProductRepository>();

            services.AddScoped<IBannerService, BannerService>();
            services.AddScoped<IBannerRepository, BannerRepository>();

            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IBrandRepository, BrandRepository>();

            services.AddScoped<IFeaturesBannerService, FeaturesBannerService>();
            services.AddScoped<IFeaturesBannerRepository, FeaturesBannerRepository>();

            services.AddScoped<ITagService, TagService>();
            services.AddScoped<ITagRepository, TagRepository>();

            services.AddScoped<ISizeService, SizeService>();
            services.AddScoped<ISizeRepository, SizeRepository>();

            services.AddScoped<IColorService, ColorService>();             
            services.AddScoped<IColorRepository, ColorRepository>();

            services.AddScoped<IReclamBannerRepository, ReclamBannerRepository>();
            services.AddScoped<IReclamBannerService, ReclamBannerService>();

            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<IBasketService, BasketService>();

            services.AddScoped<IAuthorService, AuthorService>();
            services.AddScoped<IAuthorRepository, AuthorRepository>();

            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IBlogRepository, BlogRepository>();

            //services.AddScoped<IPaymentService, PaymentService>();

            services.AddScoped<IContactService, ContactService>();

            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<ICommentService, CommentService>();

            services.AddScoped<IOrderRepository, OrderRepository>();
            services.AddScoped<IOrderService, OrderService>();

            services.AddScoped<IWishListRepository, WishListRepository>();
            services.AddScoped<IWishListService, WishListService>();

            services.AddScoped<IBlogCommentRepository, BlogCommentRepository>();
            services.AddScoped<IBlogCommentService,BlogCommnetService>();

            services.AddScoped<IMessageRepository, MessageRepository>();
            services.AddScoped<IMessageService, MessageService>();

            services.AddScoped<IAboutRepository, AboutRepository>();
            services.AddScoped<IAboutService, AboutService>();

            services.AddScoped<ICompareRepository, CompareRepository>();
            services.AddScoped<ICompareService, CompareService>();

            services.AddScoped<ICurrencyService, CurrencyService>();
        }
    }
}
