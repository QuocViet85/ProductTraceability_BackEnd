using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using App.Areas.NhaMay.Models;
using App.Areas.SanPham.Models;
using App.Database;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace App.Areas.LoSanPham.Models;

[Table("tblLoSanPham")]
public class LoSanPhamModel
{
    [Key]
    [BindNever]
    public Guid LSP_Id { set; get; }
    public string? LSP_MaLSP { set; get; }
    public Guid? LSP_SP_Id { set; get; }
    public string? LSP_Ten { set; get; }
    public DateTime? LSP_NgaySanXuat { set; get; }
    public DateTime? LSP_NgayHetHan { set; get; }
    public int? LSP_SoLuong { set; get; }
    public string? LSP_TrangThai { set; get; }
    public Guid? LSP_NM_Id { set; get; }
    public string? LSP_JsonData { set; get; }

    [BindNever]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime LSP_NgayTao { set; get; }

    [BindNever]
    public DateTime? LSP_NgaySua { set; get; }

    [BindNever]
    public Guid LSP_NguoiTao_Id { set; get; }

    [BindNever]
    public Guid? LSP_NguoiSua_Id { set; get; }

    [ForeignKey("LSP_SP_Id")]
    [BindNever]
    public SanPhamModel? LSP_SP { set; get; }

    [ForeignKey("LSP_NguoiTao_Id")]
    [BindNever]
    public AppUser? LSP_NguoiTao { set; get; }

    [ForeignKey("LSP_NguoiSua_Id")]
    [BindNever]
    public AppUser? LSP_NguoiSua { set; get; }

    [ForeignKey("LSP_NM_Id")]
    [BindNever]
    public NhaMayModel? LSP_NM { set; get; }


}