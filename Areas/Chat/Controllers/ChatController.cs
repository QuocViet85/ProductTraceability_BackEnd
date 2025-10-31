using System.Security.Claims;
using App.Areas.Chat.Cache;
using App.Areas.Chat.Model;
using App.Areas.Chat.Services;
using App.Areas.Files.Services;
using App.Areas.Files.ThongTin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace App.Areas.Chat.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IHubContext<ChatHub> _hubContext;

    private readonly IFileService _fileService;

    private IUserOnlineService _userOnlineService;

    public ChatController(IHubContext<ChatHub> hubContext, IFileService fileService, IUserOnlineService userOnlineService)
    {
        _hubContext = hubContext;
        _userOnlineService = userOnlineService;
        _fileService = fileService;
    }

    [HttpPost("message-image/{receivedUserId}")]
    public async Task<IActionResult> SendMessageImage(Guid receivedUserId, IFormFile file)
    {
        var sendUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var sendUserName = User.FindFirst("Name")?.Value;

        Console.WriteLine(file);

        _fileService.ValidateFiles(new List<IFormFile>() { file });

        var tenFile = _fileService.TaoTenFile(Path.GetExtension(file.FileName));

        var duongDanFile = _fileService.LayDuongDanFile(tenFile, ThongTinFile.KieuFile.IMAGE, KieuTaiNguyen.MESSAGE_IMAGE, Guid.Parse(sendUserId));

        using (FileStream fileStream = new FileStream(duongDanFile, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        if (_userOnlineService.IsUserOnline(receivedUserId))
        {
            await _hubContext.Clients.User(receivedUserId.ToString()).SendAsync("ReceiveMessage", sendUserId, sendUserName, tenFile, KieuTaiNguyen.MESSAGE_IMAGE);
        }
        else
        {
            var messageModel = new MessageModel();
            messageModel.SendUserId = Guid.Parse(sendUserId);
            messageModel.SendUserName = sendUserName;
            messageModel.ReceiveUserId = receivedUserId;
            messageModel.Content = tenFile;
            messageModel.TypeMessage = KieuTaiNguyen.MESSAGE_IMAGE;
            messageModel.TimeSend = DateTime.Now;

            MessageCache.ListMessagesWaitSend.Add(messageModel);
        }
        return Ok(tenFile);
    }

    [HttpDelete("message-image/{sendUserId}")]
    public async Task<IActionResult> DeleteMessageImage(Guid sendUserId, string tenFile)
    {
        var filePath = _fileService.LayDuongDanFile(tenFile, ThongTinFile.KieuFile.IMAGE, KieuTaiNguyen.MESSAGE_IMAGE, sendUserId);

        if (System.IO.File.Exists(filePath))
        {
            System.IO.File.Delete(filePath);
        }

        return Ok();
    }
}