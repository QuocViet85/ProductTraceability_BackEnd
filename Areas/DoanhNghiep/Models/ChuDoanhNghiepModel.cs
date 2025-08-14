using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Database;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace App.Areas.DoanhNghiep.Models;

[Table("tblChuDoanhNghiep")]
public class ChuDoanhNghiepModel
{
    [Key]
    [BindNever]
    public Guid CDN_Id { set; get; }

    [Required]
    public Guid CDN_DN_Id { set; get; }

    [Required]
    public Guid CDN_ChuDN_Id { set; get; }

    [ForeignKey("CDN_DN_Id")]
    [BindNever]
    public DoanhNghiepModel CDN_DN { set; get; }

    [ForeignKey("CDN_ChuDN_Id")]
    [BindNever]
    public AppUser CDN_ChuDN { set; get; }

    [BindNever]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CDN_NgayTao { set; get; }

    [BindNever]
    public Guid? CDN_NguoiTao_Id { set; get; }

    [BindNever]
    [ForeignKey("CDN_NguoiTao_Id")]
    public AppUser? CDN_NguoiTao { set; get; }
}