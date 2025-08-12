using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Areas.DoanhNghiep.Models;
using App.Areas.SanPham.Models;
using App.Database;
using App.Messages;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace App.Areas.DanhMuc.Models;

[Table("tblDanhMuc")]
public class DanhMucModel
{
    [Key]
    [BindNever]
    public Guid DM_Id { set; get; }

    [DisplayName("Tên danh mục")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public string DM_Ten { set; get; }
    public bool? DM_LaDMCha { set; get; }
    public string? DM_MoTa { set; get; }
    public Guid? DM_DMCha_Id { set; get; }

    [BindNever]
    public Guid DM_NguoiTaoId { set; get; }

    [ForeignKey("DM_NguoiTaoId")]
    [BindNever]
    public AppUser DM_NguoiTao { set; get; }

    [BindNever]
    public DateTime DM_NgayTao { set; get; }

    [BindNever]
    public DateTime? DM_NgaySua { set; get; }

    [BindNever]
    public Guid? DM_NguoiSuaId { set; get; }

    [BindNever]
    public AppUser? DM_NguoiSua { set; get; }

    [ForeignKey("DM_DMCha_Id")]
    [BindNever]
    public DanhMucModel? DM_DMCha { set; get; }

    [BindNever]
    public List<DanhMucModel>? DM_List_DMCon { set; get; }
}