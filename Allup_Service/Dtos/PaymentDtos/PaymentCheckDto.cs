
using Allup_Core.Enums;
using Allup_Service.Abstractions.Dtos;

namespace Allup_Service.Dtos.PaymentDtos
{
    public class PaymentCheckDto:IDto
    {
        public string Token { get; set; } = null!;
        public int ID { get; set; }
        public PaymentStatuses STATUS { get; set; }
    }
}
