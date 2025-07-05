
using Allup_Core.Entities;
using Allup_Core.Enums;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.PaymentDtos;
using Allup_Service.Exceptions;
using Allup_Service.Service.IService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace Allup_Service.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly PaymentConfigurationDto _paymentConfigurationDto;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUrlHelper _urlHelper;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IPaymentRepository _repository;

        public PaymentService(IPaymentRepository repository, IHttpContextAccessor contextAccessor , IUrlHelper urlHelper, IHttpClientFactory httpClientFactory , PaymentConfigurationDto paymentConfigurationDto , IConfiguration configuration , IWebHostEnvironment webHostEnvironment)
        {
            _repository = repository;
            _contextAccessor = contextAccessor;
            _urlHelper = urlHelper;
            _httpClientFactory = httpClientFactory;
            _paymentConfigurationDto = paymentConfigurationDto;
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<bool> CheckPaymentAsync(PaymentCheckDto dto)
        {
            var payment = await _repository.GetAsync(x => x.ConfirmToken == dto.Token && x.ReceptId == dto.ID && x.PaymentStatus == PaymentStatuses.Pending, include: x => x.Include(x => x.Order));

            if (payment is null)
                throw new NotFoundException();

            if (dto.STATUS == PaymentStatuses.FullyPaid)
                payment.Order.IsPaid = true;

            payment.PaymentStatus = dto.STATUS;

            _repository.Update(payment);
            await _repository.SaveChangesAsync();

            return dto.STATUS == PaymentStatuses.FullyPaid;
        }


        public async Task<PaymentResponseDto> CreateAsync(PaymentCreateDto dto)
        {
            string confirmToken = Guid.NewGuid().ToString();

            UrlActionContext context = new()
            {
                Action = "CheckPayment",
                Controller = "Order",
                Values = new { Token = confirmToken },
                Protocol = _contextAccessor.HttpContext?.Request.Scheme
            };

            var redirectUrl = _urlHelper.Action(context);

           // string selectedCulture = _languageService.GetSelectedCulture();

            string amount = dto.Amount.ToString().Replace(',', '.');

                                       //  ""language"": ""{selectedCulture}"",    Bunu Dilli yeni Language , Locizer onlari yazandan sonra bunu yazariq
            string requestBody = $@"
    {{
        ""order"": {{
            ""typeRid"": ""Order_SMS"",                                            
            ""amount"": {amount},
            ""currency"": ""AZN"",
            ""description"": ""{dto.Description}"",
            ""hppRedirectUrl"": ""{redirectUrl}"",
            ""hppCofCapturePurposes"": [""Cit""]
        }}
    }}";

            var httpClient = _httpClientFactory.CreateClient("KapitalBankClient");
            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{_paymentConfigurationDto.Username}:{_paymentConfigurationDto.Password}"));
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", credentials);


            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(_paymentConfigurationDto.BaseUrl, content);

            if (!response.IsSuccessStatusCode)
                throw new Exception(response.StatusCode.ToString());

            var responseContent = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<PaymentResponseDto>(responseContent) ?? new();

            Payment payment = new()
            {
                Amount = dto.Amount,
                Description = dto.Description,
                OrderId = dto.OrderId,
                ReceptId = result.Order.Id,
                SecretId = result.Order.Secret,
                PaymentStatus = PaymentStatuses.Pending,
                ConfirmToken = confirmToken
            };

            await _repository.CreateAsync(payment);
            await _repository.SaveChangesAsync();

            result.Id = payment.Id;

            return result;
        }
    }
}
