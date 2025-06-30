
using Allup_Core.Entities;
using Allup_Service.Abstractions.Dtos;

namespace Allup_Service.Dtos.OrderDtos
{
    public class CheckoutDto:IDto
    {
        public OrderCreateDto Order { get; set; } = new();
        public List<CartItem> BasketItems { get; set; } = new();
        public decimal Total { get; set; }
    }
}
