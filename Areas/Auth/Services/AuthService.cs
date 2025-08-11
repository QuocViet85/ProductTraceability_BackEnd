
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using App.Areas.Auth.AuthorizationType;
using App.Areas.Auth.DTO;
using App.Areas.Auth.Mapper;
using App.Areas.Auth.Models;
using App.Areas.Files;
using App.Areas.Files.DTO;
using App.Areas.Files.Services;
using App.Database;
using Areas.Auth.DTO;
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
    private readonly IFileService _fileService;
    public AuthService(UserManager<AppUser> userManager, IOptions<JwtSettings> optionJwtServices, AppDBContext dbContext, IFileService fileService)
    {
        _userManager = userManager;
        _jwtSettings = optionJwtServices.Value;
        _dbContext = dbContext;
        _fileService = fileService;
    }

    public async Task RegisterAsync(RegisterDTO registerDTO)
    {
        var user = await _userManager.FindByEmailAsync(registerDTO.Email);

        if (user != null)
        {
            throw new Exception("Email đã tồn tại");
        }

        var newUser = new AppUser()
        {
            UserName = registerDTO.PhoneNumber,
            Name = registerDTO.Name,
            PhoneNumber = registerDTO.PhoneNumber,
            Email = registerDTO.Email,
            Address = registerDTO.Address,
            CreatedAt = DateTime.Now
        };

        if (registerDTO.Role == Roles.CUSTOMER)
        {
            newUser.IsActive = true;
        }
        else if (registerDTO.Role == Roles.ENTERPRISE)
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

    public async Task<(string accessToken, string refreshToken)> LoginAsync(LoginDTO loginDTO)
    {
        var user = await _userManager.FindByNameAsync(loginDTO.PhoneNumber);

        if (user == null) throw new Exception("User không tồn tại");

        var checkPassword = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

        if (!checkPassword) throw new Exception("Sai mật khẩu");

        var roles = await _userManager.GetRolesAsync(user);

        string accessToken = await GenerateAccessTokenAsync(user, roles);

        string refreshToken = await GenerateRefreshTokenAsync(user);

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

    public async Task<UserDTO> GetOneUserAsync(Guid id)
    {
        var appUser = await _userManager.Users.Where(u => u.Id == id).FirstOrDefaultAsync();

        if (appUser == null)
        {
            throw new Exception("Không tìm thấy user");
        }

        var userDTO = UserMapper.ModelToDto(appUser);

        userDTO.Role = (await _userManager.GetRolesAsync(appUser))[0];

        return userDTO;
    }

    public async Task<UserDTO> GetMyUserAsync(ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var appUser = await _userManager.Users.Where(u => u.Id.ToString() == userIdNow).FirstOrDefaultAsync();

        if (appUser == null)
        {
            throw new Exception("Không tìm thấy user");
        }

        var userDTO = UserMapper.ModelToDto(appUser);

        userDTO.Role = (await _userManager.GetRolesAsync(appUser))[0];

        return userDTO;
    }

    public async Task<string> GetAccessTokenAsync(string refreshToken)
    {
        var refreshTokenModel = await _dbContext.RefreshTokens.Where(rt => rt.Token == refreshToken).FirstOrDefaultAsync();

        if (refreshTokenModel == null) throw new Exception("Yêu cầu đăng nhập lại");

        if (refreshTokenModel.ExpireTime < DateTime.Now) throw new Exception("Token hết hạn. Yêu cầu đăng nhập lại");

        var userId = refreshTokenModel.UserId;

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            _dbContext.RefreshTokens.Remove(refreshTokenModel);
            await _dbContext.SaveChangesAsync();
            throw new Exception("Yêu cầu đăng nhập lại");
        }

        var roles = await _userManager.GetRolesAsync(user);

        string accessToken = await GenerateAccessTokenAsync(user, roles);

        return accessToken;
    }

    public async Task LogoutAsync(ClaimsPrincipal userNowFromJwt, string refreshToken)
    {
        var user = await _userManager.GetUserAsync(userNowFromJwt);

        if (user == null) throw new Exception("User không hợp lệ");

        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM RefreshTokens WHERE UserId = {0} AND Token = {1}", user.Id, refreshToken);
    }

    public async Task LogoutAllDevicesAsync(ClaimsPrincipal userNowFromJwt)
    {
        var user = await _userManager.GetUserAsync(userNowFromJwt);

        if (user == null) throw new Exception("User không hợp lệ");

        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM RefreshTokens WHERE UserId = {0}", user.Id);
    }

    public async Task UpdateAsync(ClaimsPrincipal userNowFromJwt, UpdateUserDTO userUpdateDTO)
    {
        var user = await _userManager.GetUserAsync(userNowFromJwt);

        if (user == null) throw new Exception("User không hợp lệ");

        user.Name = userUpdateDTO.Name;
        user.Email = userUpdateDTO.Email;
        user.Address = userUpdateDTO.Address;

        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            throw new Exception("Cập nhật thất bại");
        }
    }

    public async Task ChangePasswordAsync(ClaimsPrincipal userNowFromJwt, ChangePasswordDTO changePasswordDTO)
    {
        var user = await _userManager.GetUserAsync(userNowFromJwt);

        if (user == null) throw new Exception("User không hợp lệ");

        var result = await _userManager.ChangePasswordAsync(user, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);

        if (!result.Succeeded)
        {
            throw new Exception("Đổi mật khẩu thất bại. Xem lại thông tin nhập");
        }
    }

    private async Task<string> GenerateAccessTokenAsync(AppUser user, IList<string> roles)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)); // mã hóa key bằng thuật toán đối xứng

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256); // đối tượng chứa key đã được mã hóa và thuật toán để kí token bằng key

        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()), //định danh người dùng theo chuẩn chung
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), //định danh JWT theo chuẩn chung

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

    private async Task<string> GenerateRefreshTokenAsync(AppUser user)
    {
        var randomBytes = new Byte[64]; //mảng byte trống có thể chứa 64 phần tử

        var randomNumberGenerate = RandomNumberGenerator.Create(); //trình tạo số ngẫu nhiên

        randomNumberGenerate.GetBytes(randomBytes); // ghi dạng byte của số ngẫu nhiên tạo ra vào mảng bytes

        return Convert.ToBase64String(randomBytes); //convert mảng thành chuỗi
    }

    public async Task SetAvatarAsync(ClaimsPrincipal userNowFromJwt, IFormFile avatar)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        await _fileService.DeleteManyByEntityAsync(FileInformation.EntityType.USER, userIdNow, FileInformation.FileType.AVATAR);

        int result = await _fileService.UploadAsync(new List<IFormFile>() { avatar }, new FileDTO(FileInformation.FileType.AVATAR, FileInformation.EntityType.USER, userIdNow));

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Thiết lập ảnh đại diện thất bại");
        }
    }

    public async Task DeleteAvatarAsync(ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        int result = await _fileService.DeleteManyByEntityAsync(FileInformation.EntityType.USER, userIdNow, FileInformation.FileType.AVATAR);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu hoặc không có ảnh đại diện để xóa. Xóa ảnh đại diện thất bại");
        }

    }
}