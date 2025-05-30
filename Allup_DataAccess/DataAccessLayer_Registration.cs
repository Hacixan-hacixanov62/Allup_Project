using Allup_DataAccess.DAL;
using Allup_DataAccess.Repositories;
using Allup_DataAccess.Repositories.IRepositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Allup_DataAccess
{
    public static class DataAccessLayer_Registration
    {

        public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(configuration.GetConnectionString("Default")));


            services.AddMemoryCache();

            AddRepository(services);


            return services;
        }



        private static void AddRepository(IServiceCollection service)
        {
            service.AddScoped<ISliderRepository, SliderRepository>();


        }
    }
}
