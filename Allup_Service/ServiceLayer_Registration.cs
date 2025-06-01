using Allup_DataAccess.Repositories;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Profiles;
using Allup_Service.Service;
using Allup_Service.Service.IService;
using Microsoft.AspNetCore.Builder;
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


        } 

        private static void AddServices(IServiceCollection services)
        {
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


        }
    }
}
