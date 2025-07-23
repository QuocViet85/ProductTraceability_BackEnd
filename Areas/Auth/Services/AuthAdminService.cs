using System.Security.Claims;
using Areas.Auth.DTO.Admin;
using Database;
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

    public async Task Create(UserDTO userDTO)
    {
        if (userDTO != null)
        {
            var userExist = await _userManager.FindByNameAsync(userDTO.PhoneNumber);

            if (userExist != null)
            {
                throw new Exception("User đã tồn tại");
            }

            var newUser = ConvertUserDTOToAppUser(userDTO);

            var result = await _userManager.CreateAsync(newUser, userDTO.Password);

            if (!result.Succeeded)
            {
                throw new Exception("Tạo user thất bại");
            }

            await _userManager.AddToRoleAsync(newUser, userDTO.Role);
        }
    }

    public async Task<(int totalUsers, List<UserDTO> listUsers)> GetAll(int pageNumber, int limit)
    {
        IQueryable<AppUser> queryAppUser = _userManager.Users;
        if (pageNumber > 0 && limit > 0)
        {
            queryAppUser = queryAppUser.Skip((pageNumber - 1) * limit).Take(limit);
        }

        List<AppUser> listAppUsers = await queryAppUser.ToListAsync();

        int totalUsers = await queryAppUser.CountAsync();

        List<UserDTO> listUserDTOs = new List<UserDTO>();

        if (listAppUsers?.Count > 0)
        {
            foreach (var appUser in listAppUsers)
            {
                listUserDTOs.Add(await ConvertAppUserToUserDTO(appUser));
            }
        }

        return (totalUsers, listUserDTOs);
    }

    public async Task<UserDTO> GetOne(string id)
    {
        var appUser = await _userManager.Users.Where(u => u.Id == id).FirstOrDefaultAsync();

        if (appUser == null)
        {
            throw new Exception("Không tìm thấy user");
        }

        return await ConvertAppUserToUserDTO(appUser);
    }

    public async Task Delete(string id, ClaimsPrincipal userNowFromJwt)
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
    public async Task Update(string id, UserDTO userDTO, ClaimsPrincipal userNowFromJwt)
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

        appUserUpdate = ConvertUserDTOToAppUser(userDTO, appUserUpdate);

        var resultUpdateInfo = await _userManager.UpdateAsync(appUserUpdate);

        if (!resultUpdateInfo.Succeeded)
        {
            throw new Exception("Cập nhật thông tin user thất bại");
        }

        await _userManager.AddToRoleAsync(appUserUpdate, userDTO.Role);
    }

    private AppUser ConvertUserDTOToAppUser(UserDTO userDTO, AppUser appUserUpdate = null)
    {
        AppUser appUser;
        if (appUserUpdate == null)
        {
            appUser = new AppUser();
        }
        else
        {
            appUser = appUserUpdate;
            appUser.PasswordHash = _passwordHasher.HashPassword(appUserUpdate, userDTO.Password);
        }

        appUser.UserName = userDTO.UserName;
        appUser.PhoneNumber = userDTO.PhoneNumber;
        appUser.NormalizedUserName = userDTO.PhoneNumber;
        appUser.Email = userDTO.Email;
        appUser.Address = userDTO.Address;


        if (userDTO.IsActive == null)
        {
            appUser.IsActive = true;
        }
        else
        {
            appUser.IsActive = (bool)userDTO.IsActive;
        }

        if (userDTO.PhoneNumberConfirmed == null)
        {
            appUser.PhoneNumberConfirmed = false;
        }
        else
        {
            appUser.PhoneNumberConfirmed = (bool)userDTO.PhoneNumberConfirmed;
        }

        return appUser;
    }

    private async Task<UserDTO> ConvertAppUserToUserDTO(AppUser appUser)
    {
        var userDTO = new UserDTO()
        {
            Id = appUser.Id,
            UserName = appUser.UserName,
            PhoneNumber = appUser.PhoneNumber,
            Email = appUser.Email,
            IsActive = appUser.IsActive,
            Address = appUser.Address,
            PhoneNumberConfirmed = appUser.PhoneNumberConfirmed,
        };

        userDTO.Role = (await _userManager.GetRolesAsync(appUser))[0];

        return userDTO;
    }
}