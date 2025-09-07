using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Allup_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public ChatController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] ChatRequest request)
        {
            try
            {
                Console.WriteLine("SendMessage action-a daxil oldu");

                var apiKey = _configuration["OpenAI:ApiKey"];
                Console.WriteLine($"API Key: {(!string.IsNullOrEmpty(apiKey) ? "Təyin edilib" : "Təyin edilməyib")}");

                if (string.IsNullOrEmpty(apiKey))
                {
                    return StatusCode(500, new { error = "API key təyin edilməyib", details = "Zəhmət olmasa appsettings.json faylında OpenAI:ApiKey təyin edin" });
                }

                var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

                var requestBody = new
                {
                    model = "deepseek-chat",
                    messages = new[]
                    {
                    new { role = "user", content = request.Message }
                },
                    max_tokens = 500,
                    temperature = 0.7
                };

                var json = JsonSerializer.Serialize(requestBody);
                Console.WriteLine($"Göndərilən JSON: {json}");

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                Console.WriteLine("DeepSeek API-ə sorğu göndərilir...");
                var response = await client.PostAsync("https://api.deepseek.com/v1/chat/completions", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API Cavabı: {response.StatusCode} - {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new
                    {
                        error = "DeepSeek API xətası",
                        details = responseContent,
                        statusCode = response.StatusCode
                    });
                }

                using JsonDocument document = JsonDocument.Parse(responseContent);
                JsonElement root = document.RootElement;
                var messageContent = root.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

                Console.WriteLine("Cavab uğurla alındı");
                return Ok(new { message = messageContent });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Xəta baş verdi: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                return StatusCode(500, new
                {
                    error = "Daxili server xətası",
                    details = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
    }

    public class ChatRequest
    {
        public string Message { get; set; } = null!;
    }
}
