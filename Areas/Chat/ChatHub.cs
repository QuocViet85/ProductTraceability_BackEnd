using System.Security.Claims;
using App.Areas.Chat.Cache;
using App.Areas.Chat.Model;
using App.Areas.Chat.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace App.Areas.Chat;

[Authorize]
public class ChatHub : Hub
{
    private IUserOnlineService _userOnlineService;

    public ChatHub(IUserOnlineService userOnlineService)
    {
        _userOnlineService = userOnlineService;
    }

    public async Task SendMessage(Guid receivedUserId, string message, string typeMessage)
    {
        var sendUser = Context.User;
        var sendUserId = sendUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var sendUserName = sendUser.FindFirst("Name")?.Value;

        if (_userOnlineService.IsUserOnline(receivedUserId))
        {
            await Clients.User(receivedUserId.ToString()).SendAsync("ReceiveMessage", sendUserId, sendUserName, message, typeMessage);
        }
        else
        {
            var messageModel = new MessageModel();
            messageModel.SendUserId = Guid.Parse(sendUserId);
            messageModel.SendUserName = sendUserName;
            messageModel.ReceiveUserId = receivedUserId;
            messageModel.Content = message;
            messageModel.TypeMessage = typeMessage;
            messageModel.TimeSend = DateTime.Now;

            MessageCache.ListMessagesWaitSend.Add(messageModel);
        }
    }

    public override async Task OnConnectedAsync()
    {
        var userOnline = new UserChatModel();

        userOnline.UserId = Guid.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        userOnline.ConnectionId = Context.ConnectionId;

        _userOnlineService.AddUserOnline(userOnline);

        base.OnConnectedAsync();

        var listMessageWaitSendOfUser = MessageCache.ListMessagesWaitSend.Where(m => m.ReceiveUserId == userOnline.UserId).ToList();

        foreach (var message in listMessageWaitSendOfUser)
        {
            try
            {
                await Clients.User(message.ReceiveUserId.ToString()).SendAsync("ReceiveMessage", message.SendUserId, message.SendUserName, message.Content, message.TypeMessage, message.TimeSend);
                message.ReceiveUserId = Guid.Empty;
                message.SendUserId = Guid.Empty;
                message.Content = null;
                message.TypeMessage = null;
            }
            catch
            { }  
        }
    }
    
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        var userOffline = new UserChatModel();

        userOffline.UserId = Guid.Parse(Context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        userOffline.ConnectionId = Context.ConnectionId;

        _userOnlineService.RemoveUserOnline(userOffline);

        return base.OnDisconnectedAsync(exception);
    }
}