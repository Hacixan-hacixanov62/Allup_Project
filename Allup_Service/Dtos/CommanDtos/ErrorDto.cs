using Allup_Service.Abstractions.Dtos;

namespace Allup_Service.Dtos.CommanDtos
{
    public class ErrorDto : IDto
    {
        public string Name { get; set; } = "Xəta baş verdi";
        public string Message { get; set; } = null!;
        public int StatusCode { get; set; }
    }
}
