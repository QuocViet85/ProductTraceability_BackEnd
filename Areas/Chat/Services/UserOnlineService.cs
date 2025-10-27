
using App.Areas.Chat.Model;

namespace App.Areas.Chat.Services;

public class UserOnlineService : IUserOnlineService
{
    private readonly List<UserChatModel> ListUserOnline = new List<UserChatModel>();

    public void AddUserOnline(UserChatModel userChat)
    {
        ListUserOnline.Add(userChat);
    }

    public void RemoveUserOnline(UserChatModel userChat)
    {
        ListUserOnline.RemoveAll(uo => uo.UserId == userChat.UserId && uo.ConnectionId == userChat.ConnectionId);
    }

    public bool IsUserOnline(Guid userId)
    {
        return ListUserOnline.Any(uo => uo.UserId == userId);
    }

    
}