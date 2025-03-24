using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
    
    public async Task SendPublicKey(string key)
    {
        await Clients.All.SendAsync("ReceivePublicKey", key);
    }
    
    public async Task RequestPublicKey()
    {
        await Clients.All.SendAsync("RequestKey");
    }
}