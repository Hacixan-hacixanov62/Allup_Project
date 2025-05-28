using Allup_DataAccess.Repositories;
using Allup_DataAccess.Repositories.IRepositories;
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
        public static void AddBussniessRepository(this IServiceCollection service)
        {


            AddRepository(service);
        }




        private static void AddRepository(IServiceCollection service)
        {
            service.AddScoped<ISliderRepository, SliderRepository>();


        }
    }
}
