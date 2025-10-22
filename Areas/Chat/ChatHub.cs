using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace App.Areas.Chat;

[Authorize]
public class ChatHub : Hub
{
    public async Task SendMessage(Guid receivedUserId, string message)
    {
        var sendUser = Context.User;
        var sendUserId = sendUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(sendUserId))
        {
            return;
        }

        await Clients.User(receivedUserId.ToString()).SendAsync("ReceiveMessage", sendUserId, message);
    }
}