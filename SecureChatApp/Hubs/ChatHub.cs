using Microsoft.AspNetCore.SignalR;

namespace SecureChatApp.Hubs;

public class ChatHub : Hub
{
    public async Task SendMessage(string chatRoom, string message, string userName)
    {
        await Clients.Client().SendAsync("ReceiveMessage", userName,  message);
    }
}