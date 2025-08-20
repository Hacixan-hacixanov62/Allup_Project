using Allup_Service.Dtos.HubDtos;
using Allup_Service.StaticFiles;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Allup_Service.Hubs
{
    public class ChatHub:Hub
    {
        public static List<ConnectionDto> Connections = [];
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ChatHub(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task SendMessage(string connectionId, string message)
        {
            await Clients.All.SendAsync("SendMessage", message);
        }


        public override Task OnConnectedAsync()
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var connection = HubDatas.Connections.FirstOrDefault(x => x.UserId == userId);
            if (connection is { })
            {
                connection.ConnectionIds.Add(Context.ConnectionId);
            }
            else
            {
                HubDatas.Connections.Add(new() { UserId = userId!, ConnectionIds = [Context.ConnectionId] });
            }
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            HubDatas.Connections.RemoveAll(x => x.UserId == userId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
