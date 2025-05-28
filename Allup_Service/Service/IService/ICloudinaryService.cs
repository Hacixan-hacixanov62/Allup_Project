using Microsoft.AspNetCore.Http;


namespace Allup_Service.Service.IService
{
    public interface ICloudinaryService
    {
        Task<string> FileCreateAsync(IFormFile file);
        Task<bool> FileDeleteAsync(string filePath);
    }
}
