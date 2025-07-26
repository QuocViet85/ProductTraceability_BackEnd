using System.Security.Claims;
using App.Areas.Auth.AuthorizationType;
using App.Areas.Enterprises.Auth.Edit;
using App.Areas.Enterprises.DTO;
using App.Areas.Enterprises.Models;
using App.Messages;
using App.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using App.Areas.Enterprises.Repositories;

namespace App.Areas.Enterprises.Services;

public class EnterpriseService : IEnterpriseService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IAuthorizationService _authorizationService;
    private readonly IEnterpriseRepository _enterpriseRepo;

    public EnterpriseService(UserManager<AppUser> userManager, IAuthorizationService authorizationService, IEnterpriseRepository enterpriseRepo)
    {
        _userManager = userManager;
        _authorizationService = authorizationService;
        _enterpriseRepo = enterpriseRepo;
    }

    public async Task<(int totalEnterprises, List<EnterpriseDTO> listEnterpriseDTOs)> GetManyAsync(int pageNumber, int limit, string search)
    {
        int totalEnterprises = await _enterpriseRepo.GetTotalAsync();

        List<EnterpriseModel> listEnterprises = await _enterpriseRepo.GetManyAsync(pageNumber, limit, search);

        List<EnterpriseDTO> listEnterpriseDTOs = new List<EnterpriseDTO>();

        foreach (var enterprise in listEnterprises)
        {
            listEnterpriseDTOs.Add(await ConvertModelToDTOAsync(enterprise));
        }

        return (totalEnterprises, listEnterpriseDTOs);
    }
    public async Task<(int totalEnterprises, List<EnterpriseDTO> listEnterpriseDTOs)> GetMyManyAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        int totalEnterprises = await _enterpriseRepo.GetMyTotalAsync(userIdNow);

        List<EnterpriseModel> listEnterprises = await _enterpriseRepo.GetMyManyAsync(userIdNow, pageNumber, limit, search);

        List<EnterpriseDTO> listEnterpriseDTOs = new List<EnterpriseDTO>();

        foreach (var enterprise in listEnterprises)
        {
            listEnterpriseDTOs.Add(await ConvertModelToDTOAsync(enterprise));
        }

        return (totalEnterprises, listEnterpriseDTOs);
    }

    public async Task<EnterpriseDTO> GetOneAsync(Guid id)
    {
        var enterprise = await _enterpriseRepo.GetOneAsync(id);
        if (enterprise == null)
        {
            throw new Exception("Không tìm thấy doanh nghiệp");
        }

        return await ConvertModelToDTOAsync(enterprise);
    }

    public async Task CreateAsync(EnterpriseDTO enterpriseDTO, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdNow))
        {
            throw new Exception("User không hợp lệ");
        }

        if (await _enterpriseRepo.CheckExistAsync(enterpriseDTO.TaxCode, enterpriseDTO.GLNCode))
        {
            throw new Exception("Không thể tạo doanh nghiệp vì mã số thuế hoặc mã GLN đã tồn tại");
        }

        var enterprise = ConvertDTOToModel(enterpriseDTO);

        int result = await _enterpriseRepo.CreateAsync(enterprise, userIdNow);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Tạo doanh nghiệp thất bại");
        }

        var enterpriseUser = new EnterpriseUserModel()
        {
            EnterpriseId = enterprise.Id,
            UserId = userIdNow,
            CreatedBy = true
        };

        await _enterpriseRepo.AddOwnershipAsync(enterpriseUser);
    }

    public async Task DeleteAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value; ;

        var enterprise = await _enterpriseRepo.GetOneAsync(id);

        if (enterprise == null)
        {
            throw new Exception("Doanh nghiệp không tồn tại");
        }

        var checkCanDelete = await _authorizationService.AuthorizeAsync(userNowFromJwt, enterprise, new CanEditEnterpriseRequirement(delete: true));

        if (checkCanDelete.Succeeded)
        {
            int result = await _enterpriseRepo.DeleteAsync(enterprise);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa doanh nghiệp thất bại");
            }
        }
        else
        {
            throw new Exception(ErrorMessage.AuthFailReason(checkCanDelete.Failure.FailureReasons));
        }
    }

    public async Task UpdateAsync(Guid id, EnterpriseDTO enterpriseDTO, ClaimsPrincipal userNowFromJwt)
    {
        var enterprise = await _enterpriseRepo.GetOneAsync(id);

        if (enterprise == null)
        {
            throw new Exception("Doanh nghiệp không tồn tại");
        }

        if (await _enterpriseRepo.CheckExistExceptThisAsync(id, enterpriseDTO.TaxCode, enterpriseDTO.GLNCode))
        {
            throw new Exception("Không thể sửa doanh nghiệp vì mã số thuế hoặc mã GLN đã tồn tại");
        }

        var checkCanUpdate = await _authorizationService.AuthorizeAsync(userNowFromJwt, enterprise, new CanEditEnterpriseRequirement());

        if (checkCanUpdate.Succeeded)
        {
            enterprise = ConvertDTOToModel(enterpriseDTO, enterprise);

            int result = await _enterpriseRepo.UpdateAsync(enterprise);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật doanh nghiệp thất bại");
            }
        }
        else
        {
            throw new Exception(ErrorMessage.AuthFailReason(checkCanUpdate.Failure.FailureReasons));
        }
    }

    public async Task AddOwnerShipAsync( Guid id, string userId, ClaimsPrincipal userNowFromJwt)
    {
        bool wasOwner = await _enterpriseRepo.CheckIsOwner(id, userId);
        if (wasOwner)
        {
            throw new Exception("User này đang sở hữu doanh nghiệp này");
        }

        var userAdd = await _userManager.FindByIdAsync(userId);
        if (userAdd == null)
        {
            throw new Exception("Không tìm thấy User để thêm sở hữu");
        }

        var enterprise = await _enterpriseRepo.GetOneAsync(id);
        if (enterprise == null)
        {
            throw new Exception("Doanh nghiệp không tồn tại");
        }

        var checkCanUpdate = await _authorizationService.AuthorizeAsync(userNowFromJwt, enterprise, new CanEditEnterpriseRequirement());

        if (checkCanUpdate.Succeeded)
        {
            string roleUserAdd = (await _userManager.GetRolesAsync(userAdd))[0];

            if (roleUserAdd == Roles.ADMIN || roleUserAdd == Roles.ENTERPRISE)
            {
                var enterpriseUser = new EnterpriseUserModel()
                {
                    EnterpriseId = enterprise.Id,
                    UserId = userAdd.Id
                };

                int result = await _enterpriseRepo.AddOwnershipAsync(enterpriseUser);

                if (result == 0)
                {
                    throw new Exception("Lỗi cơ sở dữ liệu. Không thể cập nhật sở hữu doanh nghiệp");
                }
            }
            else
            {
                throw new Exception($"User với vai trò {roleUserAdd} không được sở hữu doanh nghiệp");
            }
        }
        else
        {
            throw new Exception(ErrorMessage.AuthFailReason(checkCanUpdate.Failure.FailureReasons));
        }
    }

    public async Task GiveUpOwnershipAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        int result = await _enterpriseRepo.GiveUpOwnershipAsync(id, userIdNow);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Không thể từ bỏ sở hữu doanh nghiệp");
        }
    }
    public async Task DeleteOwnershipAsync(Guid id, string userId)
    {
        int result = await _enterpriseRepo.DeleteOwnershipAsync(id, userId);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Không thể xóa sở hữu doanh nghiệp");
        }
    }

    private EnterpriseModel ConvertDTOToModel(EnterpriseDTO enterpriseDTO, EnterpriseModel enterpriseUpdate = null)
    {
        EnterpriseModel enterprise;
        if (enterpriseUpdate == null)
        {
            enterprise = new EnterpriseModel();
        }
        else
        {
            enterprise = enterpriseUpdate;
        }

        enterprise.Name = enterpriseDTO.Name;
        enterprise.TaxCode = enterpriseDTO.TaxCode;
        enterprise.GLNCode = enterpriseDTO.GLNCode;
        enterprise.Address = enterpriseDTO.Address;
        enterprise.PhoneNumber = enterpriseDTO.PhoneNumber;
        enterprise.Email = enterpriseDTO.Email;
        enterprise.Type = enterpriseDTO.Type;

        return enterprise;
    }

    private async Task<EnterpriseDTO> ConvertModelToDTOAsync(EnterpriseModel enterprise)
    {
        var enterpriseDTO = new EnterpriseDTO()
        {
            Id = enterprise.Id,
            Name = enterprise.Name,
            TaxCode = enterprise.TaxCode,
            GLNCode = enterprise.GLNCode,
            Address = enterprise.Address,
            PhoneNumber = enterprise.PhoneNumber,
            Email = enterprise.Email,
            Type = enterprise.Type
        };

        if (enterprise.EnterpriseUsers != null)
        {
            enterpriseDTO.Owners = new List<EnterpriseUserDTO>();
            foreach (var enterpriseUser in enterprise.EnterpriseUsers)
            { 
                //Thêm người sở hữu công ty vào DTO để hiển thị ra view
                if (enterpriseUser.User != null)
                {
                    enterpriseDTO.Owners.Add(new EnterpriseUserDTO()
                    {
                        Id = enterpriseUser.User.Id,
                        UserName = enterpriseUser.User.UserName,
                        Role = (await _userManager.GetRolesAsync(enterpriseUser.User))[0],
                        CreatedBy = enterpriseUser.CreatedBy
                    });
                }
            }
        }

        return enterpriseDTO;
    }
}