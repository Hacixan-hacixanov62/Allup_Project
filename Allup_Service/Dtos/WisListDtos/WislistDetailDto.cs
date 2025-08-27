using Allup_Core.Entities;
using Allup_Service.Dtos.ProductDtos;

namespace Allup_Service.Dtos.WisListDtos
{
    public class WislistDetailDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public int Count { get; set; }
        public int ProductId { get; set; }
        public  List<ProductGetDto> Products { get; set; } = new();
        public List<WishlistItemCard> WishlistItems { get; set; } = new();
    }
}
