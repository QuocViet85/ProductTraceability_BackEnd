using App.Areas.Auth.Models;
using App.Areas.LoSanPham.Models;
using App.Areas.DanhMuc.Models;
using App.Areas.BinhLuan.Models;
using App.Areas.DoanhNghiep.Models;
using App.Areas.NhaMay.Models;
using App.Areas.Files.Models;
using App.Areas.SanPham.Models;
using App.Areas.SuKienTruyXuat.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace App.Database;

public class AppDBContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

    public DbSet<RefreshTokenModel> RefreshTokens { set; get; }
    public DbSet<DoanhNghiepModel> DoanhNghieps { set; get; }
    public DbSet<ChuDoanhNghiepModel> ChuDoanhNghieps { set; get; }
    public DbSet<DanhMucModel> DanhMucs { set; get; }
    public DbSet<NhaMayModel> NhaMays { set; get; }
    public DbSet<SanPhamModel> SanPhams { set; get; }
    public DbSet<BinhLuanModel> BinhLuans { set; get; }
    public DbSet<LoSanPhamModel> LoSanPhams { set; get; }
    public DbSet<SuKienTruyXuatModel> SuKienTruyXuats { set; get; }
    public DbSet<FileModel> Files { set; get; }
    public DbSet<SaoSanPhamModel> SaoSanPhams { set; get; }

    public DbSet<TheoDoiDoanhNghiepModel> TheoDoiDoanhNghieps { set; get; }
}