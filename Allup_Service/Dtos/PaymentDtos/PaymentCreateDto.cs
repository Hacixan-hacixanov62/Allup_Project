
using Allup_Service.Abstractions.Dtos;

namespace Allup_Service.Dtos.PaymentDtos
{
    public class PaymentCreateDto:IDto
    {
        public decimal Amount { get; set; }
        public string Description { get; set; } = null!;
        public int OrderId { get; set; }
    }
}
