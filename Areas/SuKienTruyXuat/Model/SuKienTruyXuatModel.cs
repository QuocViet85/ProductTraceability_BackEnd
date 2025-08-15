using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Areas.LoSanPham.Models;
using App.Database;
using App.Messages;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace App.Areas.SuKienTruyXuat.Models;

[Table("tblSuKienTruyXuat")]
public class SuKienTruyXuatModel
{
    [Key]
    [BindNever]
    public Guid SK_Id { set; get; }
    public Guid? SK_LSP_Id { set; get; }

    [DisplayName("Tên sự kiện")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public string SK_Ten { set; get; }
    public string? SK_MaSK { set; get; }
    public string? SK_MoTa { set; get; }
    public string? SK_DiaDiem { set; get; }

    [DisplayName("Ngày truy xuất")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public DateTime SK_ThoiGian { set; get; }

    [BindNever]
    public Guid? SK_NguoiTao_Id { set; get; }

    [BindNever]
    public Guid? SK_NguoiSua_Id { set; get; }

    [BindNever]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime SK_NgayTao { set; get; }

    [BindNever]
    public DateTime? SK_NgaySua { set; get; }

    [ForeignKey("SK_LSP_Id")]
    [BindNever]
    public LoSanPhamModel? SK_LSP { set; get; }

    [ForeignKey("SK_NguoiTao_Id")]
    [BindNever]
    public AppUser? SK_NguoiTao { set; get; }

    [ForeignKey("SK_NguoiSua_Id")]
    [BindNever]
    public AppUser? SK_NguoiSua { set; get; }
}