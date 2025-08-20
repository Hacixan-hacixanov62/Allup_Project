using Allup_Core.Entities;
using Allup_Service.Dtos.MessageDtos;
using Allup_Service.Hubs;
using Allup_Service.Service.IService;
using Allup_Service.StaticFiles;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Allup_Project.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly IMessageService _messageService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<ChatHub> _chatHubContext;

        public MessageController(IMessageService messageService, UserManager<AppUser> userManager, IHubContext<ChatHub> chatHubContext)
        {
            _messageService = messageService;
            _userManager = userManager;
            _chatHubContext = chatHubContext;
        }


        public async  Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)?? "";

            var user = await _userManager.FindByIdAsync(userId);
            if(user is null)
            {
                return BadRequest();
            }

            if (userId == null) return Unauthorized();

            var chats = await _messageService.GetUserChatsAsync(userId);
            return View(chats);
        }

        [Authorize]
        public async Task<IActionResult> Detail(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var chat = await _messageService.GetChatDetailAsync(id, userId);
            if (chat == null) return NotFound();

            return View(chat);
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromBody] MessageSendDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var message = await _messageService.SendMessageAsync(userId, dto);
            if (message == null) return NotFound();

            // Send message via SignalR
            var chat = await _messageService.GetChatDetailAsync(dto.ChatId, userId);
            foreach (var userChat in chat.AppUserChats.Where(m => m.AppUserId != userId))
            {
                var connectionDto = HubDatas.Connections.FirstOrDefault(m => m.UserId == userChat.AppUserId);
                foreach (var connection in connectionDto?.ConnectionIds ?? [])
                {
                    await _chatHubContext.Clients.Client(connection).SendAsync("SendMessage", new
                    {
                        text = message.Text,
                        createdTime = message.CreatedAt
                    });
                }
            }

            return Json(new
            {
                text = message.Text,
                createdTime = message.CreatedAt
            });
        }


    }
}
