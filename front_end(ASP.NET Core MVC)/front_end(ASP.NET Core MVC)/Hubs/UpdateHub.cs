using Microsoft.AspNetCore.SignalR;

namespace front_end_ASP.NET_Core_MVC_.Hubs
{
    public class UpdateHub : Hub
    {
        public async Task SendUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveUpdate", message);
        }
    }
}
