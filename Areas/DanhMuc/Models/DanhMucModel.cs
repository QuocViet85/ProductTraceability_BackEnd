using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Database;
using App.Messages;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace App.Areas.DanhMuc.Models;

[Table("tblDanhMuc")]
public class DanhMucModel
{
    [Key]
    public Guid DM_Id { set; get; }

    [DisplayName("Tên danh mục")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public string DM_Ten { set; get; }
    public string? DM_MoTa { set; get; }
    public Guid? DM_DMCha_Id { set; get; }

    [ForeignKey("DM_DMCha_Id")]
    [BindNever]
    public DanhMucModel? DM_DMCha { set; get; }

    [BindNever]
    public List<DanhMucModel>? DM_List_DMCon { set; get; }

    [BindNever]
    public Guid? DM_NguoiTao_Id { set; get; }

    [ForeignKey("DM_NguoiTao_Id")]
    [BindNever]
    public AppUser? DM_NguoiTao { set; get; }

    [BindNever]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime DM_NgayTao { set; get; }

    [BindNever]
    public DateTime? DM_NgaySua { set; get; }

    [BindNever]
    public Guid? DM_NguoiSua_Id { set; get; }

    [ForeignKey("DM_NguoiSua_Id")]
    [BindNever]
    public AppUser? DM_NguoiSua { set; get; }
}