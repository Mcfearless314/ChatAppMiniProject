using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

public class ChatHub : Hub
{
    public async Task SendMessage(string message)
    {
        var user = Context.User.Identity.Name;
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}