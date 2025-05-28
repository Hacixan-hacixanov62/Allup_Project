using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Allup_Service.Service.IService
{
    public interface  IFileService
    {
        void Delete(string fileName, string folder);
        Task<string> UploadFilesAsync(IFormFile file, string folder);
    }
}
