
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using App.Areas.Auth;
using App.Areas.Auth.DTO;
using App.Areas.Auth.Models;
using Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace App.Areas.Auth.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly JwtSettings _jwtSettings;
    private readonly AppDBContext _dbContext;
    public AuthService(UserManager<AppUser> userManager, IOptions<JwtSettings> optionJwtServices, AppDBContext dbContext)
    {
        _userManager = userManager;
        _jwtSettings = optionJwtServices.Value;
        _dbContext = dbContext;
    }

    public async Task Register(RegisterDTO registerDTO)
    {
        var user = await _userManager.FindByEmailAsync(registerDTO.Email);

        if (user != null)
        {
            throw new Exception("Email đã tồn tại");
        }

        var newUser = new AppUser()
        {
            UserName = registerDTO.UserName,
            NormalizedUserName = registerDTO.PhoneNumber,
            PhoneNumber = registerDTO.PhoneNumber,
            Email = registerDTO.Email,
            Address = registerDTO.Address,
            CreatedAt = DateTime.Now
        };

        if (registerDTO.Role == Roles.CUSTOMER)
        {
            newUser.IsActive = true;
        }
        else if (registerDTO.Role == Roles.ENTERPRISE || registerDTO.Role == Roles.SELLER)
        {
            newUser.IsActive = false;
        }
        else
        {
            throw new Exception("Vai trò không hợp lệ");
        }

        var result = await _userManager.CreateAsync(newUser, registerDTO.Password); //tự động tạo password hash

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(newUser, registerDTO.Role);
        }
        else
        {
            foreach (var error in result.Errors)
            {
                Console.WriteLine(error.Description);
            }
        }
    }

    public async Task<(string accessToken, string refreshToken)> Login(LoginDTO loginDTO)
    {
        var user = await _userManager.FindByNameAsync(loginDTO.PhoneNumber);

        if (user == null) throw new Exception("User không tồn tại");

        var checkPassword = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

        if (!checkPassword) throw new Exception("Sai mật khẩu");

        var roles = await _userManager.GetRolesAsync(user);

        string accessToken = await GenerateAccessToken(user, roles);

        string refreshToken = await GenerateRefreshToken(user);

        var refreshTokenModel = new RefreshTokenModel()
        {
            Token = refreshToken,
            UserId = user.Id,
            ExpireTime = DateTime.Now.AddDays(_jwtSettings.RefreshTokenDays)
        };

        await _dbContext.RefreshTokens.AddAsync(refreshTokenModel);
        await _dbContext.SaveChangesAsync();

        return (accessToken, refreshToken);
    }

    public async Task<string> GetAccessToken(string refreshToken)
    {
        var refreshTokenModel = await _dbContext.RefreshTokens.Where(rt => rt.Token == refreshToken).FirstOrDefaultAsync();

        if (refreshTokenModel == null) throw new Exception("Yêu cầu đăng nhập lại");

        if (refreshTokenModel.ExpireTime < DateTime.Now) throw new Exception("Token hết hạn. Yêu cầu đăng nhập lại");

        var userId = refreshTokenModel.UserId;

        var user = await _userManager.FindByIdAsync(userId);

        if (user == null)
        {
            _dbContext.RefreshTokens.Remove(refreshTokenModel);
            await _dbContext.SaveChangesAsync();
            throw new Exception("Yêu cầu đăng nhập lại");
        }

        var roles = await _userManager.GetRolesAsync(user);

        string accessToken = await GenerateAccessToken(user, roles);

        return accessToken;
    }

    public async Task Logout(ClaimsPrincipal userNowFromJwt, string refreshToken)
    {
        var user = await _userManager.GetUserAsync(userNowFromJwt);

        if (user == null) throw new Exception("User không hợp lệ");

        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM RefreshTokens WHERE UserId = {0} AND Token = {1}", user.Id, refreshToken);
    }

    public async Task LogoutAllDevices(ClaimsPrincipal userNowFromJwt)
    {
        var user = await _userManager.GetUserAsync(userNowFromJwt);

        if (user == null) throw new Exception("User không hợp lệ");

        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM RefreshTokens WHERE UserId = {0}", user.Id);
    }

    public async Task Update(ClaimsPrincipal userNowFromJwt, UpdateUserDTO userUpdateDTO)
    {
        var user = await _userManager.GetUserAsync(userNowFromJwt);

        if (user == null) throw new Exception("User không hợp lệ");

        user.UserName = userUpdateDTO.UserName;
        user.Email = userUpdateDTO.Email;
        user.Address = userUpdateDTO.Address;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            throw new Exception("Cập nhật thất bại");
        }
    }

    public async Task ChangePassword(ClaimsPrincipal userNowFromJwt, ChangePasswordDTO changePasswordDTO)
    {
        var user = await _userManager.GetUserAsync(userNowFromJwt);

        if (user == null) throw new Exception("User không hợp lệ");

        var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);

        if (!result.Succeeded)
        {
            throw new Exception("Đổi mật khẩu thất bại. Xem lại thông tin nhập");
        }
    }

    private async Task<string> GenerateAccessToken(AppUser user, IList<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)); // mã hóa key bằng thuật toán đối xứng

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // đối tượng chứa key đã được mã hóa và thuật toán để kí token bằng key

        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id), //định danh người dùng theo chuẩn chung
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //định danh JWT theo chuẩn chung
            new Claim(ClaimTypes.NameIdentifier, user.Id), //định danh người dùng theo chuẩn riêng của .NET Core (dựa vào đây để tìm người dùng bằng JWT)

            //Phục vụ hiển thị thông tin user ở client
            new Claim("PhoneNumber", user.PhoneNumber),
            new Claim("UserName", user.UserName),
            new Claim("Email", user.Email),
            new Claim("Address", user.Address)
        };

        if (roles != null)
        {
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        var tokenObj = new JwtSecurityToken
        (
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(_jwtSettings.AccessTokenMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenObj);
    }

    private async Task<string> GenerateRefreshToken(AppUser user)
    {
        var randomBytes = new Byte[64]; //mảng byte trống có thể chứa 64 phần tử

        var randomNumberGenerate = RandomNumberGenerator.Create(); //trình tạo số ngẫu nhiên

        randomNumberGenerate.GetBytes(randomBytes); // ghi dạng byte của số ngẫu nhiên tạo ra vào mảng bytes

        return Convert.ToBase64String(randomBytes); //convert mảng thành chuỗi
    }
}