using Allup_Core.Entities;
using Allup_DataAccess.Repositories.IRepositories;
using Allup_Service.Dtos.MessageDtos;
using Allup_Service.Service.IService;
using Microsoft.AspNetCore.Identity;

namespace Allup_Service.Service
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly UserManager<AppUser> _userManager;

        public MessageService(UserManager<AppUser> userManager, IMessageRepository messageRepository)
        {
            _userManager = userManager;
            _messageRepository = messageRepository;
        }

        public async Task<Chat?> GetChatDetailAsync(int chatId, string userId)
        {
            return await _messageRepository.GetChatWithUsersAndMessagesAsync(chatId,userId);
        }

        public async Task<List<Chat>> GetUserChatsAsync(string userId)
        {
           return await _messageRepository.GetUserChatsAsync(userId);
        }

        public async Task<Message?> SendMessageAsync(string userId, MessageSendDto dto)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            var chat = await _messageRepository.GetChatWithUsersAndMessagesAsync(dto.ChatId, userId);
            if (chat == null) return null;

            var message = new Message
            {
                Text = dto.Text,
                ChatId = dto.ChatId,
                SenderId = userId,
                CreatedAt = DateTime.UtcNow
            };

            return await _messageRepository.AddMessageAsync(message);
        }
    }
}
