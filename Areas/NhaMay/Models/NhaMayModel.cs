using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Areas.DoanhNghiep.Models;
using App.Database;
using App.Messages;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace App.Areas.NhaMay.Models;

[Table("tblNhaMay")]
public class NhaMayModel
{
    [Key]
    [BindNever]
    public Guid NM_Id { set; get; }

    [DisplayName("Tên nhà máy")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public string NM_Ten { set; get; }
    public string? NM_MaNM { set; get; }
    public string? NM_DiaChi { set; get; }
    public string? NM_LienHe { set; get; }

    [BindNever]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime NM_NgayTao { set; get; }

    [BindNever]
    public Guid? NM_NguoiTao_Id { set; get; }

    [ForeignKey("NM_NguoiTao_Id")]
    [BindNever]
    public AppUser? NM_NguoiTao { set; get; }
    public Guid? NM_DN_Id { set; get; }

    [ForeignKey("NM_DN_Id")]
    [BindNever]
    public DoanhNghiepModel? NM_DN { set; get; }

    [BindNever]
    public DateTime? NM_NgaySua { set; get; }

    [BindNever]
    public Guid? NM_NguoiSua_Id { set; get; }

    [ForeignKey("NM_NguoiSua_Id")]
    [BindNever]
    public AppUser? NM_NguoiSua { set; get; }
}