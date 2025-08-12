
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Database;
using App.Messages;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace App.Areas.DoanhNghiep.Models;

[Table("tblDoanhNghiep")]
public class DoanhNghiepModel
{
    [Key]
    [BindNever]
    public Guid DN_Id { set; get; }

    [DisplayName("Tên công ty")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public string DN_Ten { set; get; }

    [DisplayName("Mã số thuế")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public string DN_MaSoThue { set; get; }

    [DisplayName("Mã GLN")]
    [StringLength(13, ErrorMessage = "{0} chỉ có thể có 13 kí tự")]
    public string? DN_MaGLN { set; get; }
    public string? DN_DiaChi { set; get; }
    public string? DN_SoDienThoai { set; get; }
    public string? DN_Email { set; get; }
    public int? DN_KieuDN { set; get; }
    public string? DN_JsonData { set; get; }

    [BindNever]
    public DateTime DN_NgayTao { set; get; }

    [BindNever]
    public Guid? DN_NguoiTaoId { set; get; }

    [BindNever]
    public AppUser? DN_NguoiTao { set; get; }

    [BindNever]
    public DateTime? DN_NgaySua { set; get; }

    [BindNever]
    public Guid? DN_NguoiSuaId { set; get; }

    [ForeignKey("DN_NguoiSuaId")]
    [BindNever]
    public AppUser? DN_NguoiSua { set; get; }

    [BindNever]
    public List<ChuDoanhNghiepModel>? DN_List_CDN { set; get; }
}