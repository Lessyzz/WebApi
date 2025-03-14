using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace WebApi.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            var organizationId = Context.User?.Claims.FirstOrDefault(c => c.Type == "orgid");
            
            if (organizationId !=null)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, organizationId.Value);
            }

            await base.OnConnectedAsync();
        }
        
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var organizationId = Context.User?.Claims.FirstOrDefault(c => c.Type == "orgid");
            
            if (organizationId !=null)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, organizationId.Value);
            }

            await base.OnDisconnectedAsync(exception);
        }
        
        [HubMethodName("send_message")]
        [Authorize]
        public void SendMessage(string message)
        {
            Console.WriteLine($"Socket - Received: {message}");
        }
    }
}
