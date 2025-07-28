using App.Database;
using Areas.Auth.DTO;

namespace App.Areas.Auth.Mapper;

public static class UserMapper 
{
    public static AppUser DtoToModel(UserDTO userDTO, AppUser appUserUpdate = null)
    {
        AppUser appUser;
        if (appUserUpdate == null)
        {
            appUser = new AppUser();
        }
        else
        {
            appUser = appUserUpdate;
        }

        appUser.UserName = userDTO.PhoneNumber;
        appUser.PhoneNumber = userDTO.PhoneNumber;
        appUser.Name = userDTO.Name;
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

    public static UserDTO ModelToDto(AppUser appUser)
    {
        var userDTO = new UserDTO()
        {
            Id = appUser.Id,
            Name = appUser.Name,
            PhoneNumber = appUser.PhoneNumber,
            Email = appUser.Email,
            IsActive = appUser.IsActive,
            Address = appUser.Address,
            PhoneNumberConfirmed = appUser.PhoneNumberConfirmed,
            CreatedAt = appUser.CreatedAt
        };

        return userDTO;
    }
}