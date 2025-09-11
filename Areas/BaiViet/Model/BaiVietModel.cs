using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Areas.SanPham.Models;
using App.Messages;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace App.Areas.BaiViet.Model;

[Table("tblBaiViet")]
public class BaiVietModel
{
    [Key]
    public Guid BV_Id { set; get; }

    [DisplayName("Nội dung bài viết")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public string BV_TieuDe { set; get; }

    [DisplayName("Nội dung bài viết")]
    [Required(ErrorMessage = ErrorMessage.Required)]
    public string BV_NoiDung { set; get; }

    public Guid? BV_SP_Id { set; get; }

    [BindNever]
    public Guid BV_NguoiTao_Id { set; get; }

    [BindNever]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime BV_NgayTao { set; get; }

    public DateTime? BV_NgaySua { set; get; }
    
    [ForeignKey("BV_SP_Id")]
    public SanPhamModel? BV_SP { set; get; }
}