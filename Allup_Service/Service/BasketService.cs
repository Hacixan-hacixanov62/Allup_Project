using Allup_Core.Entities;
using Allup_DataAccess.DAL;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.CartDtos;
using Allup_Service.Dtos.ColorDtos;
using Allup_Service.Dtos.ProductDtos;
using Allup_Service.Exceptions;
using Allup_Service.Service.IService;
using Allup_Service.UI.Vm;
using AutoMapper;
using CloudinaryDotNet.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Reflection;
using System.Security.Claims;

namespace Allup_Service.Service
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProductService _productService;
        private readonly AppDbContext _context;
        public const string BASKET_KEY = "AllupCart";
        public BasketService(IMapper mapper, IHttpContextAccessor httpContextAccessor, IBasketRepository basketRepository, IProductService productService, AppDbContext context)
        {
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _basketRepository = basketRepository;
            _productService = productService;
            _context = context;
        }

        public async Task<bool> AddToCartAsync(int id, int count = 1)
        {
            var isExistProduct = await _productService.IsExistAsync(id);

            if (isExistProduct is false)
                throw new NotFoundException("Notfound Basket Product");


            if (count < 1)
                count = 1;

            if (_checkAuthorized())
            {
                string userId = _getUserId();

                var existCartItem = await _basketRepository.GetAsync(x => x.ProductId == id && x.AppUserId == userId);

                if (existCartItem is { })
                {
                    existCartItem.Count += count;

                    _basketRepository.Update(existCartItem);
                    await _basketRepository.SaveChangesAsync();

                    return true;
                }

                CartItem cartItem = new() { AppUserId = userId, ProductId = id, Count = count };

                await _basketRepository.CreateAsync(cartItem);
                await _basketRepository.SaveChangesAsync();

                return true;
            }
            else
            {
                var cartItems = _readCartFromCookie();

                var existItem = cartItems.FirstOrDefault(x => x.ProductId == id);

                if (existItem is { })
                    existItem.Count += count;
                else
                {
                    CartItem newCartItem = new() { ProductId = id, Count = count };

                    cartItems.Add(newCartItem);
                }

                _writeCartInCookie(cartItems);

                return true;
            }
        }

        public async Task ClearCartAsync()
        {
            if (!_checkAuthorized())
            {
                _writeCartInCookie(new());
                return;
            }

            string userId = _getUserId();

            var cartItems = await _basketRepository.GetFilter(x => x.AppUserId == userId).ToListAsync();

            foreach (var cartItem in cartItems)
            {
                await _basketRepository.Delete(cartItem);
            }

            await _basketRepository.SaveChangesAsync();
        }

        public async Task<bool> DecreaseToCartAsync(int id)
        {
            var isExistProduct = await _productService.IsExistAsync(id);

            if (isExistProduct is false)
                throw new NotFoundException("Notfound Product");

            if (_checkAuthorized())
            {
                string userId = _getUserId();

                var existCartItem = await _basketRepository.GetAsync(x => x.ProductId == id && x.AppUserId == userId);

                if (existCartItem is null)
                    throw new NotFoundException("Notfound Product");

                if (existCartItem.Count <= 1)
                    return true;

                existCartItem.Count--;

                _basketRepository.Update(existCartItem);
                await _basketRepository.SaveChangesAsync();

                return true;

            }
            else
            {
                var cartItems = _readCartFromCookie();

                var existItem = cartItems.FirstOrDefault(x => x.ProductId == id);

                if (existItem is null)
                    throw new NotFoundException("Notfound Product");

                if (existItem.Count <= 1)
                    return true;

                existItem.Count--;

                _writeCartInCookie(cartItems);

                return true;
            }
        }

        public async Task<CartGetDto> GetCartAsync()
        {

            if (_checkAuthorized())
            {
                var userId = _getUserId();



                var cartItems = await _basketRepository.GetFilter(x => x.AppUserId == userId,
                               x => x.Include(x => x.Product)
                                          .Include(x => x.Product.Category)
                                          .Include(x => x.Product.ProductImages)).ToListAsync() ?? new List<CartItem>();

                var dtos = _mapper.Map<List<CartItemDto>>(cartItems);

                decimal subtotal = dtos.Sum(x => x.Count * x.Product.SalePrice);
                decimal discount = subtotal * 0.20m;
                decimal total = subtotal - discount;

                var cartDto = new CartGetDto()
                {
                    Items = dtos,
                    Count = dtos.Count,
                    Subtotal = subtotal,
                    Total = total,
                    Discount = discount,
                };

                return cartDto;
            }
            else
            {
                var cartItems = _readCartFromCookie();

                var dtos = _mapper.Map<List<CartItemDto>>(cartItems);

                foreach (var dto in dtos)
                {
                    var product = await _productService.GetByIdAsync(dto.ProductId);

                    if (product is null)
                    {
                        dtos.Remove(dto);
                        continue;
                    }

                    dto.Product = product;
                }

                decimal totalPrice = dtos.Sum(x => (x.Count * x.Product.SalePrice));
                var cartDto = new CartGetDto()
                {
                    Items = dtos,
                    Count = dtos.Count,
                    Subtotal = totalPrice,
                    Total = totalPrice,
                    Discount = 0,
                };

                return cartDto;
            }
        }

        public async Task<int> GetIntAsync()
        {
            if (_checkAuthorized())
            {
                var userId = _getUserId();
                var count = await _context.CartItems
                                          .Where(x => x.AppUserId == userId)
                                          .Select(x => x.ProductId)
                                          .Distinct()
                                          .CountAsync(); // Eyni məhsulu bir dəfə say
                return count;
            }

            var request = _httpContextAccessor.HttpContext.Request;
            if (request.Cookies[BASKET_KEY] != null)
            {
                var basketItems = JsonConvert.DeserializeObject<List<CartItem>>(request.Cookies[BASKET_KEY]);
                var distinctCount = basketItems.Select(x => x.ProductId)
                                               .Distinct()
                                               .Count();
                return distinctCount;
            }

            return 0;
        }

        public CartGetDto GetUserBasketItem()
        {
            CartGetDto cartGetDto = new CartGetDto();

            if (_checkAuthorized())
            {
                var userId = _getUserId();

                var items = _context.CartItems
                    .Include(x => x.Product)
                    .ThenInclude(x => x.ProductImages.Where(x => x.IsCover == true)) // .ThenInclude(x => x.Product.ColorProducts)
                    .Where(x => x.AppUserId == userId).ToList();

                foreach (var bi in items)
                {

                    CartItemDto basketItemVM = new CartItemDto()
                    {
                        Count = bi.Count,

                        MainImage = bi.Product.ProductImages
                .Where(x => x.IsCover == true)
                .Select(x => x.ImageUrl) // <-- Make sure this matches your actual property
                .FirstOrDefault(),
                        Product = new ProductGetDto
                        {
                            Id = bi.Product.Id,
                            Name = bi.Product.Name,
                            SalePrice = bi.Product.SalePrice,
                            DiscountPercent = bi.Product.DiscountPercent,
                            MainFileUrl = bi.Product.ProductImages
                .Where(x => x.IsCover == true)
                .Select(x => x.ImageUrl) // <-- Make sure this matches your actual property
                .FirstOrDefault()
                        }
                    };
                    ProductGetDto dto = _mapper.Map<ProductGetDto>(bi.Product);

                    cartGetDto.Items.Add(basketItemVM);
                    cartGetDto.Total += (basketItemVM.Product.DiscountPercent > 0 ? basketItemVM.Product.DiscountPercent : basketItemVM.Product.SalePrice) * basketItemVM.Count;

                }

            }
            else
            {
                var basketStr = _readCartFromCookie();

                List<CartItem> cookieItems = null;

                if (basketStr != null)
                    cookieItems = _readCartFromCookie();
                else
                    cookieItems = _readCartFromCookie();


                try
                {
                    foreach (var cItem in cookieItems)
                    {
                        var product = _context.Products.Include(x => x.ProductImages.Where(x => x.IsCover == true)).FirstOrDefault(x => x.Id == cItem.ProductId);

                        if (product is null)
                            continue;

                        CartItemDto basketItemVM = new CartItemDto()
                        {

                            Count = cItem.Count,
                            Product = new ProductGetDto
                            {
                                Id = product.Id,
                                Name = product.Name,                                       
                                SalePrice = product.SalePrice,                             
                                MainFileUrl = product.ProductImages.FirstOrDefault(x => x.IsCover == true)?.ImageUrl,
                                //Colors = product.ColorProducts.Select(cp => new ColorGetDto
                                //{
                                //    Id = cp.Color.Id,
                                //    Name = cp.Color.Name
                                //}).ToList(),
                            }
                        };


                        cartGetDto.Items.Add(basketItemVM);
                        cartGetDto.Total += (basketItemVM.Product.DiscountPercent > 0 ? basketItemVM.Product.DiscountPercent : basketItemVM.Product.SalePrice) * basketItemVM.Count;

                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine(ex.Message);
                }


             
            }

            return cartGetDto;
        }
        public Task RemoveToBasket(int id, string? returnUrl)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveToCartAsync(int id)
        {
            var isExistProduct = await _productService.IsExistAsync(id);

            if (isExistProduct is false)
                throw new NotFoundException("Notfound Product");

            if (_checkAuthorized())
            {
                string userId = _getUserId();

                var existCartItem = await _basketRepository.GetAsync(x => x.ProductId == id && x.AppUserId == userId);

                if (existCartItem is null)
                    throw new NotFoundException("Notfound Product");

                await _basketRepository.Delete(existCartItem);
                await _basketRepository.SaveChangesAsync();
            }
            else
            {
                List<CartItem> cartItems = _readCartFromCookie();

                var existItem = cartItems.FirstOrDefault(x => x.ProductId == id);

                if (existItem is null)
                    throw new NotFoundException("Notfound Product");

                cartItems.Remove(existItem);

                _writeCartInCookie(cartItems);
            }
        }


        public async Task<List<CartItem>> GetBasketAsync()
        {
            if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);

                var basketItems = await _context.CartItems.Include(x => x.Product).ThenInclude(x => x.ProductImages).Where(x => x.AppUserId == userId).ToListAsync();
                return basketItems;
            }

            var basktItms = _getBasket();
            foreach (var item in basktItms)
            {
                var product = await _context.Products.Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == item.ProductId);
                item.Product = product;


            }

            return basktItms;
        }

        public List<CartItem> _getBasket()
        {
            List<CartItem> basketItems = new();
            if (_httpContextAccessor.HttpContext.Request.Cookies["AllupCart"] != null)
            {
                basketItems = JsonConvert.DeserializeObject<List<CartItem>>(_httpContextAccessor.HttpContext.Request.Cookies["AllupCart"]) ?? new();
            }

            return basketItems;
        }



        public Task EditBasketItem(int id, int count)
        {
            throw new NotImplementedException();
        }
        public Task AddToBasket(int id, string? returnUrl, int count = 1, int page = 1)
        {
            throw new NotImplementedException();
        }

        //============= Esas sehifedeki basket funksionalliqlari "shopCart"
        public Task DeleteBasket(int id)
        {
            CartGetDto vm = new CartGetDto();
            var userId = _getUserId();

            if (userId != null)
            {
                var basketItem = _context.CartItems
                    .Include(x => x.Product)
                    .ThenInclude(p => p.ProductImages)
                    .FirstOrDefault(x => x.AppUserId == userId && x.ProductId == id);

                if (basketItem == null)
                    throw new NotFoundException("Data NotFound");

                _context.CartItems.Remove(basketItem);
                _context.SaveChanges();

                var basketItems = _context.CartItems
                    .Include(x => x.Product)
                    .ThenInclude(p => p.ProductImages)
                    .Where(x => x.AppUserId == userId)
                    .ToList();

                foreach (var item in basketItems)
                {
                    var productDto = _mapper.Map<ProductGetDto>(item.Product);

                    CartItemDto basketItemVM = new CartItemDto
                    {
                        Count = item.Count,
                        Product = productDto
                    };

                    vm.Items.Add(basketItemVM);
                    vm.Total += (productDto.DiscountPercent > 0 ? productDto.DiscountPercent : productDto.SalePrice) * basketItemVM.Count;
                }
            }
            else
            {
                var cartItems = _readCartFromCookie();
                var existItem = cartItems.FirstOrDefault(x => x.ProductId == id);

                if (existItem == null)
                    throw new NotFoundException("Data NotFound");

                cartItems.Remove(existItem);
                _writeCartInCookie(cartItems);

                foreach (var cItem in cartItems)
                {
                    var product = _context.Products
                        .Include(x => x.ProductImages)
                        .FirstOrDefault(p => p.Id == cItem.ProductId);

                    if (product == null)
                    {
                        continue;
                    }

                    var productDto = _mapper.Map<ProductGetDto>(product);

                    CartItemDto basketItemVM = new CartItemDto
                    {
                        Count = cItem.Count,
                        Product = productDto
                    };

                    vm.Items.Add(basketItemVM);
                    vm.Total += (productDto.DiscountPercent > 0 ? productDto.DiscountPercent : productDto.SalePrice) * basketItemVM.Count;
                }
            }

            return Task.CompletedTask;
        }


        private List<CartItem> _readCartFromCookie()
        {
            string json = _httpContextAccessor.HttpContext?.Request.Cookies[BASKET_KEY] ?? "";

            var cartItems = JsonConvert.DeserializeObject<List<CartItem>>(json) ?? new();
            return cartItems;
        }

        private void _writeCartInCookie(List<CartItem> cartItems)
        {
            string newJson = JsonConvert.SerializeObject(cartItems);

            _httpContextAccessor.HttpContext?.Response.Cookies.Append(BASKET_KEY, newJson);
        }


        private string _getUserId()
        {
            return _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        private bool _checkAuthorized()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }


    }
}
