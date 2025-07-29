

using Allup_Core.Entities;

namespace Allup_Service.Dtos.PaginateDto
{
    public class PaginateListDto
    {
        public List<Order> Orders { get; set; } = new();
        public int CurrentPage { get; set; }
        public int PageCount { get; set; }
    }
}
