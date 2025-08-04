using App.Areas.Auth.Models;
using App.Areas.Batches.Models;
using App.Areas.Categories.Models;
using App.Areas.Comments.Models;
using App.Areas.Enterprises.Models;
using App.Areas.Factories.Models;
using App.Areas.IndividualEnterprises.Model;
using App.Areas.Products.Models;
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
    public DbSet<CategoryModel> Categories { set; get; }
    public DbSet<FactoryModel> Factories { set; get; }
    public DbSet<IndividualEnterpriseModel> IndividualEnterprises { set; get; }
    public DbSet<ProductModel> Products { set; get; }
    public DbSet<CommentModel> Comments { set; get; }
    public DbSet<BatchModel> Batches { set; get; }
}