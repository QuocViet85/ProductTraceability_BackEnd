namespace App.Areas.Chat.Model;

public class MessageModel
{
    public Guid SendUserId { set; get; }
    public string SendUserName { set; get; }
    public Guid ReceiveUserId { set; get; }
    public string Content { set; get; }
    public DateTime TimeSend { set; get; }
    public string TypeMessage { set; get; }
}