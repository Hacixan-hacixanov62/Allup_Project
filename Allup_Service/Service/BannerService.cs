using Allup_Core.Entities;
using Allup_Service.Dtos.BannerDtos;
using Allup_Service.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Service.Service
{
    public class BannerService : IBannerService
    {
        public Task CreateAsync(BannerCreateDto bannerCreateDto)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Banner> DetailAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task EditAsync(int id, BannerUpdateDto bannerUpdateDto)
        {
            throw new NotImplementedException();
        }

        public Task<List<Banner>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<BannerGetDto> GetByIdAsync(int bannerId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsExistAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
