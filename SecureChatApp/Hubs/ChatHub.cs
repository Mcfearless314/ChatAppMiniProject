using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

public class ChatHub : Hub
{
    public async Task SendMessage(string user, string ciphertext, string iv)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, ciphertext, iv);
    }
    
    public async Task SendPublicKey(string publicKey)
    {
        await Clients.AllExcept(Context.ConnectionId).SendAsync("ReceivePublicKey", publicKey);
    }
    
    public async Task RequestPublicKey()
    {
        await Clients.All.SendAsync("RequestKey");
    }
}