using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.OrderDtos;
using Allup_Service.Dtos.PaginateDto;
using Allup_Service.Exceptions;
using Allup_Service.Service.IService;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Allup_Service.Service
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly AppDbContext _context;

        public OrderService(AppDbContext context, IHttpContextAccessor httpContextAccessor , IMapper mapper , IOrderRepository orderRepository )
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _orderRepository = orderRepository;
        }

        public async Task CancelOrderAsync(int id)
        {
            var order = await _orderRepository.GetAsync(id, x => x.Include(x => x.OrderItems));

            if (order is null)
                throw new NotFoundException("NotFound Order");

            order.IsCanceled = !order.IsCanceled;

            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();
        }
        public async Task<bool> CreateAsync(OrderCreateDto dto, ModelStateDictionary ModelState)
        {
            if (!ModelState.IsValid)
                return false;

            var order = _mapper.Map<Order>(dto);

            string? userName = _httpContextAccessor.HttpContext?.User.Identity?.Name;

            // `CreatedBy` və `UpdatedBy` doldurulur
            order.CreatedBy = userName ?? "Unknown";
            order.UpdatedBy = userName ?? "Unknown";
            order.CreatedAt = DateTime.UtcNow;
            //var status = await _statusService.GetFirstAsync();
            //order.StatusId = status.Id;

            string userId = _getUserId()!;
            order.AppUserId = userId;

            await _orderRepository.CreateAsync(order);
            await _orderRepository.SaveChangesAsync();

            //foreach (var item in dto.OrderItems)
            //    await _productService.IncreaseSalesCountAsync(item.ProductId, item.Count);

            return true;
        }

        private string? _getUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? null;
        }

        public async Task<Order> DetailAsync(int id)
        {
            var order = await _orderRepository.GetAll()
                                        .Include(m => m.OrderItems)
                                        .ThenInclude(x => x.Product)
                                        .ThenInclude(x => x.ProductImages)
                                        .FirstOrDefaultAsync(m => m.Id == id);
            await _orderRepository.SaveChangesAsync();
            if (order == null)
            {
                throw new NotFoundException("Order NotFound");
            }

            return order;

        }


        public IQueryable<Order> GetAll()
        {
            return _orderRepository.GetAll();
        }
        public async Task<List<Order>> GetAllAsync()
        {
            return await _orderRepository.GetAll()
                                          .Include(m => m.OrderItems)
                                          .ToListAsync();
        }

        public Order GetLastOrderForUser(string userId)
        {
            return _context.Orders
            .Where(x => x.AppUserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .FirstOrDefault() ?? new Order();
        }

        public async Task NextOrderStatusAsync(int id)
        {
            var order = await _orderRepository.GetAsync(id, x => x.Include(x => x.OrderItems));

            if (order is null)
                throw new NotFoundException("Order NotFound");

            if (order.Status == false)
                order.Status = null;
            else
                order.Status = true;

            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();
        }

        public async Task PrevOrderStatusAsync(int id)
        {
            var order = await _orderRepository.GetAsync(id, x => x.Include(x => x.OrderItems));

            if (order is null)
                throw new NotFoundException("NotFound Order");

            if (!order.IsCanceled)
            {
                if (order.Status is true)
                    order.Status = null;
                else
                    order.Status = false;

                _orderRepository.Update(order);
                await _orderRepository.SaveChangesAsync();
            }


        }

        public async Task RepairOrderAsync(int id)
        {
            var order = await _orderRepository.GetAsync(id, x => x.Include(x => x.OrderItems));

            if (order is null)
                throw new NotFoundException("NotFound Order");
            order.IsCanceled = !order.IsCanceled;

            _orderRepository.Update(order);
            await _orderRepository.SaveChangesAsync();

        }

        public async Task<PaginateListDto> GetPaginatedAsync(int page, int take)
        {
            int totalOrders = await _orderRepository.GetAll().CountAsync();
            int pageCount = (int)Math.Ceiling((decimal)totalOrders / take);

            if (pageCount == 0)
                pageCount = 1;

            if (page > pageCount)
                page = pageCount;

            if (page <= 0)
                page = 1;

            var orders = await _orderRepository
                                .GetAll()
                                .OrderByDescending(x => x.CreatedAt)
                                .Skip((page - 1) * take)
                                .Take(take)
                                .Include(x => x.AppUser)
                                .ToListAsync();

            return new PaginateListDto
            {
                Orders = orders,
                CurrentPage = page,
                PageCount = pageCount
            };
        }
    }
}
