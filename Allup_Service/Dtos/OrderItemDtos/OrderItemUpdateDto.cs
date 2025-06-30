using Allup_Service.Dtos.ProductDtos;

namespace Allup_Service.Dtos.OrderItemDtos
{
    public class OrderItemUpdateDto
    {
        public int ProductId { get; set; }
        public ProductGetDto Product { get; set; } = null!;
        public int Count { get; set; }
    }
}
