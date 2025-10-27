using App.Areas.Chat.Model;

namespace App.Areas.Chat.Cache;

public static class MessageCache
{
    public static readonly List<MessageModel> ListMessagesWaitSend = new List<MessageModel>();
}