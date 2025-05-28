using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System.Net;
using Allup_Service.Dtos.Cloudinary;
using Allup_Service.Service.IService;
using Microsoft.Extensions.Configuration;

namespace Allup_Service.Service
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly IConfiguration _configuration;
        private readonly CloudinaryOptionsDto _optionsDto;
        private readonly Cloudinary _cloudinary = null!;

        public CloudinaryService(IConfiguration configuration)
        {
            _configuration = configuration;
            //   _optionsDto = _configuration.GetSection("CloudinarySettings").Get<CloudinaryOptionsDto>() ?? new();

            //var myAccount = new Account { ApiKey = _optionsDto.APIKey, ApiSecret = _optionsDto.APISecret, Cloud = _optionsDto.CloudName };

            //_cloudinary = new Cloudinary(myAccount);
            //_cloudinary.Api.Secure = true;                 // Get<CloudinaryOptionsDto>() error verdiyi ucun asagdaki kodlarnan evez eledim

            var section = _configuration.GetSection("CloudinarySettings");
            var options = new CloudinaryOptionsDto();
            section.Bind(options);
            _optionsDto = options ?? new();

            var myAccount = new Account
            {
                ApiKey = _optionsDto.APIKey,
                ApiSecret = _optionsDto.APISecret,
                Cloud = _optionsDto.CloudName
            };

            _cloudinary = new Cloudinary(myAccount);
            _cloudinary.Api.Secure = true;
        }

        public async Task<string> FileCreateAsync(IFormFile file)
        {
            string fileName = string.Concat(Guid.NewGuid(), file.FileName.Substring(file.FileName.LastIndexOf('.')));

            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(fileName, stream),
                    Folder = "AllupProject"
                };
                uploadResult = await _cloudinary.UploadAsync(uploadParams);
            }
            string url = uploadResult.SecureUrl.ToString();

            return url;
        }

        public async Task<bool> FileDeleteAsync(string filePath)
        {
            try
            {
                string publicIdWithExtension = filePath.Substring(filePath.LastIndexOf("AllupProject"));
                string publicId = publicIdWithExtension.Substring(0, publicIdWithExtension.LastIndexOf('.'));

                var deleteParams = new DelResParams()
                {
                    PublicIds = new List<string> { publicId },
                    Type = "upload",
                    ResourceType = ResourceType.Image
                };
                var result = await _cloudinary.DeleteResourcesAsync(deleteParams);

                return result.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
