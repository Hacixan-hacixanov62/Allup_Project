using Allup_Core.Enums;
using Allup_Service.Abstractions.Dtos;

namespace Allup_Service.Dtos.PaymentDtos
{
    public class PaymentGetDto:IDto
    {
        public int OrderId { get; set; }
        public int ReceptId { get; set; }
        public string SecretId { get; set; } = null!;
        public PaymentStatuses PaymentStatus { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}
