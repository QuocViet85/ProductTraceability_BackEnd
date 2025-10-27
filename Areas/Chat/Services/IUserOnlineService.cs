using App.Areas.Chat.Model;

namespace App.Areas.Chat.Services;

public interface IUserOnlineService
{
    public void AddUserOnline(UserChatModel userOnline);

    public void RemoveUserOnline(UserChatModel userOnline);

    public bool IsUserOnline(Guid userId);
}