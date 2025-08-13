using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Areas.BinhLuan.Models;
using App.Database;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using App.Areas.DanhMuc.Models;
using App.Areas.DoanhNghiep.Models;
using App.Areas.NhaMay.Models;
using App.Messages;

namespace App.Areas.SanPham.Models;

[Table("tblSanPham")]
public class SanPhamModel
{
    [Key]
    [BindNever]
    public Guid SP_Id { set; get; }

    [DisplayName("Tên sản phẩm")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public string SP_Ten { set; get; }

    [Required]
    public string SP_MaTruyXuat { set; get; }

    public string? SP_MaVach { set; get; }

    public string? SP_MoTa { set; get; }

    public string? SP_Website { set; get; }

    [Precision(18, 2)]
    public decimal? SP_Gia { set; get; }

    [Range(1, 2)]
    public string? SP_MaQuocGia { set; get; }

    [BindNever]
    public DateTime SP_NgayTao { set; get; }

    [BindNever]
    public Guid? SP_NguoiTao_Id { set; get; }

    [ForeignKey("SP_NguoiTao_Id")]
    [BindNever]
    public AppUser? SP_NguoiTao { set; get; }

    [BindNever]
    public DateTime? SP_NgaySua { set; get; }

    [BindNever]
    public Guid? SP_NguoiSua_Id { set; get; }

    [ForeignKey("SP_NguoiSua_Id")]
    [BindNever]
    public AppUser? SP_NguoiSua { set; get; }
    public Guid? SP_DM_Id { set; get; }

    [ForeignKey("SP_DM_Id")]
    [BindNever]
    public DanhMucModel? SP_DM { set; get; }

    [BindNever]
    public Guid? SP_NguoiPhuTrach_Id { set; get; }

    [ForeignKey("SP_NguoiPhuTrach_Id")]
    [BindNever]
    public AppUser? SP_NguoiPhuTrach { set; get; }

    [BindNever]
    public Guid? SP_DN_SoHuu_Id { set; get; }

    [ForeignKey("SP_DN_SoHuu_Id")]
    [BindNever]
    public DoanhNghiepModel? SP_DN_SoHuu { set; get; }

    [BindNever]
    public Guid? SP_DN_VanTai_Id { set; get; }

    [ForeignKey("DN_VanTai_Id")]
    [BindNever]
    public DoanhNghiepModel? SP_DN_VanTai { set; get; }

    [BindNever]
    public Guid? SP_DN_SanXuat_Id { set; get; }

    [ForeignKey("SP_DN_SanXuat_Id")]
    [BindNever]
    public DoanhNghiepModel? SP_DN_SanXuat { set; get; }

    [BindNever]
    public Guid? SP_NM_Id { set; get; }

    [ForeignKey("SP_NM_Id")]
    [BindNever]
    public NhaMayModel? SP_NM { set; get; }
}