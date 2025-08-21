using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace App.Areas.SanPham.Models;

[Table("tblSaoSanPham")]
public class SaoSanPhamModel
{
    [Key]
    public Guid SSP_Id { set; get; }
    public Guid SSP_NguoiTao_Id { set; get; }
    public Guid SSP_SP_Id { set; get; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime SSP_NgayTao { set; get; }
    public int SSP_SoSao { set; get; }
}