using System.Security.Claims;
using App.Areas.Auth.AuthorizationType;
using App.Areas.Enterprises.Auth;
using App.Areas.Enterprises.DTO;
using App.Areas.Enterprises.Models;
using App.Messages;
using App.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using App.Areas.Enterprises.Repositories;
using App.Areas.Enterprises.Mapper;
using App.Areas.IndividualEnterprises.Repositories;

namespace App.Areas.Enterprises.Services;

public class EnterpriseService : IEnterpriseService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IAuthorizationService _authorizationService;
    private readonly IEnterpriseRepository _enterpriseRepo;
    private readonly IIndividualEnterpiseRepository _individualEnterpriseRepo;

    public EnterpriseService(UserManager<AppUser> userManager, IAuthorizationService authorizationService, IEnterpriseRepository enterpriseRepo, IIndividualEnterpiseRepository individualEnterpiseRepo)
    {
        _userManager = userManager;
        _authorizationService = authorizationService;
        _enterpriseRepo = enterpriseRepo;
        _individualEnterpriseRepo = individualEnterpiseRepo;
    }

    public async Task<(int totalItems, List<EnterpriseDTO> listDTOs)> GetManyAsync(int pageNumber, int limit, string search, bool descending)
    {
        int totalEnterprises = await _enterpriseRepo.GetTotalAsync();

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<EnterpriseModel> listEnterprises = await _enterpriseRepo.GetManyAsync(pageNumber, limit, search, descending);

        List<EnterpriseDTO> listEnterpriseDTOs = new List<EnterpriseDTO>();

        foreach (var enterprise in listEnterprises)
        {
            var enterpriseDTO = EnterpriseMapper.ModelToDto(enterprise);
            await AddRelationToDTO(enterpriseDTO, enterprise);
            listEnterpriseDTOs.Add(enterpriseDTO);
        }

        return (totalEnterprises, listEnterpriseDTOs);
    }
    public async Task<(int totalItems, List<EnterpriseDTO> listDTOs)> GetMyManyAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search, bool descending)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        int totalMyEnterprises = await _enterpriseRepo.GetMyTotalAsync(Guid.Parse(userIdNow));

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<EnterpriseModel> listEnterprises = await _enterpriseRepo.GetMyManyAsync(Guid.Parse(userIdNow), pageNumber, limit, search, descending);

        List<EnterpriseDTO> listEnterpriseDTOs = new List<EnterpriseDTO>();

        foreach (var enterprise in listEnterprises)
        {
            var enterpriseDTO = EnterpriseMapper.ModelToDto(enterprise);
            await AddRelationToDTO(enterpriseDTO, enterprise);
            listEnterpriseDTOs.Add(enterpriseDTO);
        }

        return (totalMyEnterprises, listEnterpriseDTOs);
    }

    public async Task<EnterpriseDTO> GetOneByIdAsync(Guid id)
    {
        var enterprise = await _enterpriseRepo.GetOneByIdAsync(id);
        if (enterprise == null)
        {
            throw new Exception("Không tìm thấy doanh nghiệp");
        }

        var enterpriseDTO = EnterpriseMapper.ModelToDto(enterprise);
        await AddRelationToDTO(enterpriseDTO, enterprise);
        return enterpriseDTO;
    }

    public async Task<EnterpriseDTO> GetOneByTaxCode(string taxCode)
    {
        var enterprise = await _enterpriseRepo.GetOneByTaxCodeAsync(taxCode);
        if (enterprise == null)
        {
            throw new Exception("Không tìm thấy doanh nghiệp");
        }

        var enterpriseDTO = EnterpriseMapper.ModelToDto(enterprise);
        await AddRelationToDTO(enterpriseDTO, enterprise);
        return enterpriseDTO;
    }

    public async Task CreateAsync(EnterpriseDTO enterpriseDTO, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (await _enterpriseRepo.CheckExistByCodeAsync(enterpriseDTO.TaxCode, enterpriseDTO.GLNCode) || await _individualEnterpriseRepo.CheckExistByTaxCodeAndGLNCodeAsync(enterpriseDTO.TaxCode, enterpriseDTO.GLNCode))
        {
            throw new Exception("Không thể tạo doanh nghiệp vì mã số thuế hoặc mã GLN đã tồn tại");
        }

        var enterprise = EnterpriseMapper.DtoToModel(enterpriseDTO);

        enterprise.CreatedAt = DateTime.Now;

        int result = await _enterpriseRepo.CreateAsync(enterprise);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Tạo doanh nghiệp thất bại");
        }

        var enterpriseUser = new EnterpriseUserModel()
        {
            EnterpriseId = enterprise.Id,
            UserId = Guid.Parse(userIdNow),
            CreatedBy = true
        };

        await _enterpriseRepo.AddOwnershipAsync(enterpriseUser);
    }

    public async Task DeleteAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var enterprise = await _enterpriseRepo.GetOneByIdAsync(id);

        if (enterprise == null)
        {
            throw new Exception("Doanh nghiệp không tồn tại");
        }

        var checkCanDelete = await _authorizationService.AuthorizeAsync(userNowFromJwt, enterprise, new CanUpdateAndDeleteEnterpriseRequirement(delete: true));

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
            throw new UnauthorizedAccessException(ErrorMessage.AuthFailReason(checkCanDelete.Failure.FailureReasons));
        }
    }

    public async Task UpdateAsync(Guid id, EnterpriseDTO enterpriseDTO, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var enterprise = await _enterpriseRepo.GetOneByIdAsync(id);

        if (enterprise == null)
        {
            throw new Exception("Doanh nghiệp không tồn tại");
        }

        if (await _enterpriseRepo.CheckExistExceptThisByCodeAsync(id, enterpriseDTO.TaxCode, enterpriseDTO.GLNCode) || await _individualEnterpriseRepo.CheckExistByTaxCodeAndGLNCodeAsync(enterpriseDTO.TaxCode, enterpriseDTO.GLNCode))
        {
            throw new Exception("Không thể sửa doanh nghiệp vì mã số thuế hoặc mã GLN đã tồn tại");
        }

        var checkCanUpdate = await _authorizationService.AuthorizeAsync(userNowFromJwt, enterprise, new CanUpdateAndDeleteEnterpriseRequirement());

        if (checkCanUpdate.Succeeded)
        {
            enterprise = EnterpriseMapper.DtoToModel(enterpriseDTO, enterprise);

            enterprise.UpdatedAt = DateTime.Now;
            enterprise.UpdatedUserId = Guid.Parse(userIdNow);

            int result = await _enterpriseRepo.UpdateAsync(enterprise);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật doanh nghiệp thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException(ErrorMessage.AuthFailReason(checkCanUpdate.Failure.FailureReasons));
        }
    }

    public async Task AddOwnerShipAsync(Guid id, Guid userId, ClaimsPrincipal userNowFromJwt)
    {
        bool wasOwner = await _enterpriseRepo.CheckIsOwner(id, userId);
        if (wasOwner)
        {
            throw new Exception("User này đang sở hữu doanh nghiệp này");
        }

        var userAdd = await _userManager.FindByIdAsync(userId.ToString());
        if (userAdd == null)
        {
            throw new Exception("Không tìm thấy User để thêm sở hữu");
        }

        var enterprise = await _enterpriseRepo.GetOneByIdAsync(id);
        if (enterprise == null)
        {
            throw new Exception("Doanh nghiệp không tồn tại");
        }

        var checkCanUpdate = await _authorizationService.AuthorizeAsync(userNowFromJwt, enterprise, new CanUpdateAndDeleteEnterpriseRequirement());

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
            throw new UnauthorizedAccessException(ErrorMessage.AuthFailReason(checkCanUpdate.Failure.FailureReasons));
        }
    }

    public async Task GiveUpOwnershipAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        int result = await _enterpriseRepo.GiveUpOwnershipAsync(id, Guid.Parse(userIdNow));

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Không thể từ bỏ sở hữu doanh nghiệp");
        }
    }

    public async Task DeleteOwnershipAsync(Guid id, Guid userId)
    {
        int result = await _enterpriseRepo.DeleteOwnershipAsync(id, userId);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Không thể xóa sở hữu doanh nghiệp");
        }
    }

    private async Task AddRelationToDTO(EnterpriseDTO enterpriseDTO, EnterpriseModel enterprise)
    {
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
                        Name = enterpriseUser.User.UserName,
                        Role = (await _userManager.GetRolesAsync(enterpriseUser.User))[0],
                        CreatedBy = enterpriseUser.CreatedBy
                    });
                }
            }
        }

        if (enterprise.UpdatedUser != null)
        {
            enterpriseDTO.UpdatedUser = new EnterpriseUserDTO()
            {
                Id = enterprise.UpdatedUser.Id,
                Name = enterprise.UpdatedUser.Name,
                Role = (await _userManager.GetRolesAsync(enterprise.UpdatedUser))[0],
            };
        }
    }
}