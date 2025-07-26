using App.Areas.Auth.Models;
using App.Areas.Enterprises.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace App.Database;

public class AppDBContext : IdentityDbContext<AppUser>
{
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }

    public DbSet<RefreshTokenModel> RefreshTokens { set; get; }
    public DbSet<EnterpriseModel> Enterprises { set; get; }

    public DbSet<EnterpriseUserModel> EnterpriseUsers { set; get; }
}