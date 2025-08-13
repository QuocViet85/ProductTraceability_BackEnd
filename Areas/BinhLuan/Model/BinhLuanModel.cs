using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Areas.SanPham.Models;
using App.Database;
using App.Messages;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace App.Areas.BinhLuan.Models;

[Table("tblBinhLuan")]
public class BinhLuanModel
{
    [Key]
    [BindNever]
    public Guid Id { set; get; }

    [DisplayName("Nội dung bình luận")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public string BL_NoiDung { set; get; }

    [DisplayName("Sản phẩm")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public Guid BL_SP_Id { set; get; }


    [ForeignKey("ProductId")]
    [BindNever]
    public SanPhamModel SanPham { set; get; }

    [BindNever]
    public DateTime BL_NgayTao { set; get; }

    [Required]
    [BindNever]
    public Guid BL_NguoiTao_Id { set; get; }

    [ForeignKey("BL_NguoiTao_Id")]
    [BindNever]
    public AppUser BL_NguoiTao { set; get; }
}