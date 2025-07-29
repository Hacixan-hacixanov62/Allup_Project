using Allup_Core.Entities;
using Allup_Service.Dtos.PaginateDto;

namespace Allup_Service.Service.IService
{
    public interface IOrderService
    {
        Task CancelOrderAsync(int id);
        Task RepairOrderAsync(int id);
        Task NextOrderStatusAsync(int id);
        Task PrevOrderStatusAsync(int id);
        Task<List<Order>> GetAllAsync();
        IQueryable<Order> GetAll();
        Task<Order> DetailAsync(int id);
        Task<PaginateListDto> GetPaginatedAsync(int page, int take);
        // ==
        Order GetLastOrderForUser(string userId);

    }
}
