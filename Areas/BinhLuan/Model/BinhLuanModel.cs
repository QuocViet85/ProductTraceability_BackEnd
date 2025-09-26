using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Areas.SanPham.Models;
using App.Database;
using App.Messages;
using Areas.Auth.DTO;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace App.Areas.BinhLuan.Models;

[Table("tblBinhLuan")]
public class BinhLuanModel
{
    [Key]
    public Guid BL_Id { set; get; }

    [Required]
    public string BL_NoiDung { set; get; }

    [Required]
    public Guid BL_SP_Id { set; get; }

    [BindNever]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime BL_NgayTao { set; get; }

    [Required]
    [BindNever]
    public Guid BL_NguoiTao_Id { set; get; }

    [ForeignKey("BL_NguoiTao_Id")]
    [BindNever]
    public AppUser? BL_NguoiTao { set; get; }

    [ForeignKey("BL_SP_Id")]
    [BindNever]
    public SanPhamModel? BL_SP { set; get; }

    [BindNever]
    [NotMapped]
    public UserDTO? BL_NguoiTao_Client { set; get; }
}