using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App.Areas.Batches.DTO;
using App.Messages;
using Areas.Auth.DTO;

namespace App.Areas.TraceEvents.DTO;

public class TraceEventDTO
{
    public Guid? Id { set; get; }
    public Guid? BatchId { set; get; }

    [DisplayName("Tên sự kiện")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public string Name { set; get; }
    public string? TraceEventCode { set; get; }
    public string? Description { set; get; }
    public string? Location { set; get; }
    public DateTime? TimeStamp { set; get; }
    public Guid? CreatedUserId { set; get; }
    public Guid? UpdatedUserId { set; get; }
    public DateTime CreatedAt { set; get; }
    public DateTime? UpdatedAt { set; get; }
    public BatchDTO? Batch { set; get; }
    public UserDTO? CreatedUser { set; get; }
    public UserDTO? UpdatedUser { set; get; }
}