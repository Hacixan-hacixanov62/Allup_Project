

using Allup_Service.Dtos.PaymentDtos;

namespace Allup_Service.Service.IService
{
    public interface IPaymentService
    {
        Task<PaymentResponseDto> CreateAsync(PaymentCreateDto dto);
        Task<bool> CheckPaymentAsync(PaymentCheckDto dto);
    }
}
