using Allup_Core.Entities;
using Allup_Service.Dtos.MessageDtos;

namespace Allup_Service.Service.IService
{
    public interface IMessageService
    {
        Task<Message?> SendMessageAsync(string userId, MessageSendDto dto);
        Task<Chat?> GetChatDetailAsync(int chatId, string userId);
        Task<List<Chat>> GetUserChatsAsync(string userId);
    }
}
