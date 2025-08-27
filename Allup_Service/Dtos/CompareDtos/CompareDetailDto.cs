using Allup_Core.Entities;
using Allup_Service.Dtos.ProductDtos;

namespace Allup_Service.Dtos.CompareDtos
{
    public class CompareDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
        public string Color { get; set; } = null!;
        public int Rating { get; set; }
        public int InStock { get; set; }
        public string Title { get; set; } = null!;
        public string Desc { get; set; } = null!;
        public int ProductId { get; set; }
        public List<ProductGetDto> Products { get; set; } = new();
        public List<CompareItemCard> CompareItems { get; set; } = new();
    }
}
