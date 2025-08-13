using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Database;

namespace App.Areas.Files.Models;

[Table("tblFiles")]
public class FileModel
{
    [Key]
    public Guid F_Id { set; get; }

    [Required]
    public string F_Ten { set; get; }

    [Required]
    public string F_KieuFile { set; get; }

    public long? F_KichThuoc { set; get; }

    [Required]
    public string F_KieuTaiNguyen { set; get; }

    [Required]
    public Guid F_TaiNguyenId { set; get; }

    public Guid? F_NguoiTao_Id { set; get; }

    public DateTime F_NgayTao { set; get; }

    [ForeignKey("F_NguoiTao_Id")]
    public AppUser? F_NguoiTao { set; get; }
}