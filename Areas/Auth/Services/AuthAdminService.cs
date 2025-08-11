using System.Security.Claims;
using App.Areas.Auth.AuthorizationType;
using App.Areas.Auth.Mapper;
using App.Database;
using Areas.Auth.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Auth.Services;

public class AuthAdminService : IAuthAdminService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IPasswordHasher<AppUser> _passwordHasher;

    public AuthAdminService(UserManager<AppUser> userManager, IPasswordHasher<AppUser> passwordHasher)
    {
        _userManager = userManager;
        _passwordHasher = passwordHasher;
    }

    public async Task CreateAsync(UserDTO userDTO)
    {
        if (userDTO != null)
        {
            var userExist = await _userManager.FindByNameAsync(userDTO.PhoneNumber);

            if (userExist != null)
            {
                throw new Exception("User đã tồn tại");
            }

            var newUser = UserMapper.DtoToModel(userDTO);

            newUser.CreatedAt = DateTime.Now;

            var result = await _userManager.CreateAsync(newUser, userDTO.Password);

            if (!result.Succeeded)
            {
                throw new Exception("Tạo user thất bại");
            }
            await _userManager.AddToRoleAsync(newUser, userDTO.Role);
        }
    }

    public async Task<(int totalUsers, List<UserDTO> listUsers)> GetManyAsync(int pageNumber, int limit, string search)
    {
        IQueryable<AppUser> queryAppUser = _userManager.Users;

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryAppUser = queryAppUser.Where(u => u.UserName.Contains(search) || u.PhoneNumber.Contains(search));
        }
        
        Paginate.SetPaginate(ref pageNumber, ref limit);

        queryAppUser = queryAppUser.Skip((pageNumber - 1) * limit).Take(limit);

        List<AppUser> listAppUsers = await queryAppUser.ToListAsync();

        int totalUsers = await queryAppUser.CountAsync();

        List<UserDTO> listUserDTOs = new List<UserDTO>();

        if (listAppUsers?.Count > 0)
        {
            foreach (var appUser in listAppUsers)
            {
                var userDTO = UserMapper.ModelToDto(appUser);
                listUserDTOs.Add(userDTO);
                userDTO.Role = (await _userManager.GetRolesAsync(appUser))[0];
            }
        }

        return (totalUsers, listUserDTOs);
    }

    public async Task DeleteAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var userNow = await _userManager.GetUserAsync(userNowFromJwt);
        var appUser = await _userManager.Users.Where(u => u.Id == id).FirstOrDefaultAsync();

        if (appUser == null)
        {
            throw new Exception("Không tìm thấy user");
        }

        var role = (await _userManager.GetRolesAsync(appUser))[0];

        if (role == Roles.ADMIN && userNow.Id != id)
        {
            throw new Exception("Không được xóa Admin khác");
        }

        var result = await _userManager.DeleteAsync(appUser);

        if (!result.Succeeded)
        {
            throw new Exception("Xóa user thất bại");
        }
    }
    public async Task UpdateAsync(Guid id, UserDTO userDTO, ClaimsPrincipal userNowFromJwt)
    {
        var userNow = await _userManager.GetUserAsync(userNowFromJwt);
        var appUserUpdate = await _userManager.Users.Where(u => u.Id == id).FirstOrDefaultAsync();

        if (appUserUpdate == null)
        {
            throw new Exception("Không tìm thấy user");
        }

        var role = (await _userManager.GetRolesAsync(appUserUpdate))[0];

        if (role == Roles.ADMIN && userNow.Id != id)
        {
            throw new Exception("Không được sửa Admin khác");
        }

        appUserUpdate = UserMapper.DtoToModel(userDTO, appUserUpdate);

        appUserUpdate.PasswordHash = _passwordHasher.HashPassword(appUserUpdate, userDTO.Password);

        var resultUpdateInfo = await _userManager.UpdateAsync(appUserUpdate);

        if (!resultUpdateInfo.Succeeded)
        {
            throw new Exception("Cập nhật thông tin user thất bại");
        }
        await _userManager.AddToRoleAsync(appUserUpdate, userDTO.Role);
    }
}