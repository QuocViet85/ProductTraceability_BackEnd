using App.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;
using App.Areas.Auth;
using App.Areas.Auth.Services;
using App.Areas.DoanhNghiep.Services;
using App.Areas.DoanhNghiep.Auth;
using App.Areas.DanhMuc.Repositories;
using App.Areas.DanhMuc.Services;
using App.Areas.NhaMay.Repositories;
using App.Areas.NhaMay.Services;
using App.Areas.NhaMay.Authorization;
using App.Areas.SanPham.Repositories;
using App.Areas.SanPham.Services;
using App.Areas.SanPham.Authorization;
using App.Areas.BinhLuan.Repositories;
using App.Areas.BinhLuan.Services;
using App.Areas.LoSanPham.Repositories;
using App.Areas.LoSanPham.Services;
using App.Areas.SuKienTruyXuat.Repositories;
using App.Areas.SuKienTruyXuat.Services;
using App.Areas.Files.Services;
using App.Areas.Files.Repositories;
using App.Areas.DoanhNghiep.Repositories;
using App.Areas.BaiViet.Repositories;
using App.Areas.BaiViet.Services;


internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("AppDbContextConnection");

        Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Debug()  // Log mức debug trở lên
        .WriteTo.Console() // Log ra console
        .CreateLogger();

        builder.Host.UseSerilog();

        builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

        builder.Services.AddDbContext<AppDBContext>(options => options.UseSqlServer(connectionString));

        builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
        })
        .AddEntityFrameworkStores<AppDBContext>()
        .AddDefaultTokenProviders();

        builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("jwt"));

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var jwtSettings = builder.Configuration.GetSection("jwt").Get<JwtSettings>();
            var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

            options.TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };
        });

        builder.Services.AddAuthorization();

        builder.Services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireDigit = false; // Không bắt phải có số
            options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
            options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
            options.Password.RequireUppercase = false; // Không bắt buộc chữ in
            options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
            options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

            // Cấu hình đăng nhập.
            options.SignIn.RequireConfirmedEmail = false;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
            options.SignIn.RequireConfirmedPhoneNumber = false;     // Xác thực số điện thoại
            options.SignIn.RequireConfirmedAccount = false;         // Xác thực tài khoản
        });

        builder.Services.AddControllers();

        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IAuthAdminService, AuthAdminService>();
        builder.Services.AddScoped<IDoanhNghiepRepository, DoanhNghiepRepository>();
        builder.Services.AddScoped<IDoanhNghiepService, DoanhNghiepService>();
        builder.Services.AddScoped<IAuthorizationHandler, DoanhNghiepAuthorizationHandler>();
        builder.Services.AddScoped<IDanhMucRepository, DanhMucRepository>();
        builder.Services.AddScoped<IDanhMucService, DanhMucService>();
        builder.Services.AddScoped<INhaMayRepository, NhaMayRepository>();
        builder.Services.AddScoped<INhaMayService, NhaMayService>();
        builder.Services.AddScoped<IAuthorizationHandler, NhaMayAuthorizationHandler>();
        builder.Services.AddScoped<ISanPhamRepository, SanPhamRepository>();
        builder.Services.AddScoped<ISanPhamService, SanPhamService>();
        builder.Services.AddScoped<IAuthorizationHandler, SanPhamAuthorizationHandler>();
        builder.Services.AddScoped<IBinhLuanRepository, BinhLuanRepository>();
        builder.Services.AddScoped<IBinhLuanService, BinhLuanService>();
        builder.Services.AddScoped<ILoSanPhamRepository, LoSanPhamRepository>();
        builder.Services.AddScoped<ILoSanPhamService, LoSanPhamService>();
        builder.Services.AddScoped<ISuKienTruyXuatRepository, SuKienTruyXuatRepository>();
        builder.Services.AddScoped<ISuKienTruyXuatService, SuKienTruyXuatService>();
        builder.Services.AddScoped<IFileRepository, FileRepository>();
        builder.Services.AddScoped<IFileService, FileService>();
        builder.Services.AddScoped<IBaiVietRepository, BaiVietRepository>();
        builder.Services.AddScoped<IBaiVietService, BaiVietService>();

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        var app = builder.Build();

        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet("/weatherforecast", [Authorize] () =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast")
        .WithOpenApi();

        app.MapControllerRoute(
            name: "MyArea",
            pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

        app.Run();
    }
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}


// Lệnh chạy ứng dụng cho các thiết bị khác chung wifi truy cập được: dotnet run --urls http://0.0.0.0:5000
// Lệnh lấy domain máy tính để thiết bị dùng chung wifi truy cập được: ipconfig
// http://192.168.1.9:5000/swagger/index.html
