using Allup_Service.Service;
using Allup_Service.Service.IService;
using Microsoft.Extensions.DependencyInjection;


namespace Allup_Service
{
    public static class ServiceLayer_Registration
    {
        public static void AddBusinessServices(this IServiceCollection services)
        {
            var builder = WebApplication.CreateBuilder();

            AddServices(services);

        }


        private static void AddServices(IServiceCollection services)
        {
            services.AddScoped<ISliderService, SliderService>();
            services.AddScoped<IFileService, FileService>();
        }
    }
}
