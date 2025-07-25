using System.Security.Claims;
using App.Areas.Auth.AuthorizationType;
using App.Areas.Enterprises.Auth.Edit;
using App.Areas.Enterprises.DTO;
using App.Areas.Enterprises.Models;
using App.Messages;
using Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Enterprises.Services;

public class EnterpriseService : IEnterpriseService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDBContext _dbContext;
    private readonly IAuthorizationService _authorizationService;

    public EnterpriseService(UserManager<AppUser> userManager, AppDBContext dbContext, IAuthorizationService authorizationService)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _authorizationService = authorizationService;
    }

    public async Task<(int totalEnterprises, List<EnterpriseDTO> listEnterpriseDTOs)> GetMany(int pageNumber, int limit, string search)
    {
        IQueryable<EnterpriseModel> queryEnterprises = _dbContext.Enterprises;

        if (pageNumber > 0 && limit > 0)
        {
            queryEnterprises = queryEnterprises.Skip((pageNumber - 1) * limit).Take(limit);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryEnterprises = queryEnterprises.Where(e => e.Name.Contains(search) || e.Type.Contains(search) || e.PhoneNumber.Contains(search)); //phân tích thành SQL chứ không thực sự chạy nên NULL cũng không lỗi
        }

        int totalEnterprises = await _dbContext.Enterprises.CountAsync();

        List<EnterpriseModel> listEnterprises = await queryEnterprises.Include(e => e.EnterpriseUsers).ThenInclude(eu => eu.User).ToListAsync();

        List<EnterpriseDTO> listEnterpriseDTOs = new List<EnterpriseDTO>();

        foreach (var enterprise in listEnterprises)
        {
            listEnterpriseDTOs.Add(await ConvertModelToDTO(enterprise));
        }

        return (totalEnterprises, listEnterpriseDTOs);
    }
    public async Task<(int totalEnterprises, List<EnterpriseDTO> listEnterpriseDTOs)> GetMyMany(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search)
    {
        var userNow = await _userManager.GetUserAsync(userNowFromJwt);

        IQueryable<EnterpriseUserModel> queryEnterpriseUser = _dbContext.EnterpriseUsers.Where(eu => eu.UserId == userNow.Id);
        List<EnterpriseUserModel> listEnterpriseUser = await queryEnterpriseUser.ToListAsync();
        IQueryable<EnterpriseModel> queryEnterprises = queryEnterpriseUser.Select(eu => eu.Enterprise);

        if (pageNumber > 0 && limit > 0)
        {
            queryEnterprises = queryEnterprises.Skip((pageNumber - 1) * limit).Take(limit);
        }

        if (!string.IsNullOrEmpty(search))
        {
            search = search.Trim();
            queryEnterprises = queryEnterprises.Where(e => e.Name.Contains(search) || e.Type.Contains(search) || e.PhoneNumber.Contains(search)); //phân tích thành SQL chứ không thực sự chạy nên NULL cũng không lỗi
        }

        int totalEnterprises = await _dbContext.Enterprises.CountAsync();

        List<EnterpriseModel> listEnterprises = await queryEnterprises.ToListAsync();

        List<EnterpriseDTO> listEnterpriseDTOs = new List<EnterpriseDTO>();

        foreach (var enterprise in listEnterprises)
        {
            enterprise.EnterpriseUsers = listEnterpriseUser;
            listEnterpriseDTOs.Add(await ConvertModelToDTO(enterprise));
        }

        return (totalEnterprises, listEnterpriseDTOs);
    }

    public async Task<EnterpriseDTO> GetOne(Guid Id)
    {
        var enterprise = await _dbContext.Enterprises.Where(e => e.Id == Id).Include(e => e.EnterpriseUsers).ThenInclude(eu => eu.User).FirstOrDefaultAsync();
        if (enterprise == null)
        {
            throw new Exception("Không tìm thấy doanh nghiệp");
        }

        return await ConvertModelToDTO(enterprise);
    }

    public async Task Create(ClaimsPrincipal userNowFromJwt, EnterpriseDTO enterpriseDTO)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdNow))
        {
            throw new Exception("User không hợp lệ");
        }

        if (await _dbContext.Enterprises.AnyAsync(e => e.TaxCode == enterpriseDTO.TaxCode || e.GLNCode == enterpriseDTO.GLNCode))
        {
            throw new Exception("Không thể tạo doanh nghiệp vì mã số thuế hoặc mã GLN đã tồn tại");
        }

        var enterprise = ConvertDTOToModel(enterpriseDTO);

        await _dbContext.Enterprises.AddAsync(enterprise);
        int result = await _dbContext.SaveChangesAsync();

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

        await _dbContext.EnterpriseUsers.AddAsync(enterpriseUser);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(ClaimsPrincipal userNowFromJwt, Guid Id)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value; ;

        var enterprise = await _dbContext.Enterprises.Where(e => e.Id == Id).Include(e => e.EnterpriseUsers).FirstOrDefaultAsync();

        if (enterprise == null)
        {
            throw new Exception("Doanh nghiệp không tồn tại");
        }

        var checkCanDelete = await _authorizationService.AuthorizeAsync(userNowFromJwt, enterprise, new CanEditEnterpriseRequirement(delete: true));

        if (checkCanDelete.Succeeded)
        {
            _dbContext.Enterprises.Remove(enterprise);
            int result = await _dbContext.SaveChangesAsync();

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

    public async Task Update(ClaimsPrincipal userNowFromJwt, Guid Id, EnterpriseDTO enterpriseDTO)
    {
        var enterprise = await _dbContext.Enterprises.Where(e => e.Id == Id).Include(e => e.EnterpriseUsers).ThenInclude(eu => eu.User).FirstOrDefaultAsync();

        if (enterprise == null)
        {
            throw new Exception("Doanh nghiệp không tồn tại");
        }

        if (await _dbContext.Enterprises.AnyAsync(e => (e.TaxCode == enterpriseDTO.TaxCode && e.Id != Id) || e.GLNCode == enterpriseDTO.GLNCode && e.Id != Id))
        {
            throw new Exception("Không thể sửa doanh nghiệp vì mã số thuế hoặc mã GLN đã tồn tại");
        }

        var checkCanUpdate = await _authorizationService.AuthorizeAsync(userNowFromJwt, enterprise, new CanEditEnterpriseRequirement());

        if (checkCanUpdate.Succeeded)
        {
            enterprise = ConvertDTOToModel(enterpriseDTO, enterprise);

            _dbContext.Enterprises.Update(enterprise);
            int result = await _dbContext.SaveChangesAsync();

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

    public async Task AddOwnerShip(ClaimsPrincipal userNowFromJwt, Guid Id, string userId)
    {
        bool wasOwner = await _dbContext.EnterpriseUsers.AnyAsync(eu => eu.UserId == userId && eu.EnterpriseId == Id);
        if (wasOwner)
        {
            throw new Exception("User này đang sở hữu doanh nghiệp này");
        }

        var userAdd = await _userManager.FindByIdAsync(userId);
        if (userAdd == null)
        {
            throw new Exception("Không tìm thấy User để thêm sở hữu");
        }

        var enterprise = await _dbContext.Enterprises.Where(e => e.Id == Id).Include(e => e.EnterpriseUsers).ThenInclude(eu => eu.User).FirstOrDefaultAsync();
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

                await _dbContext.EnterpriseUsers.AddAsync(enterpriseUser);
                int result = await _dbContext.SaveChangesAsync();

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

    public async Task GiveUpOwnership(ClaimsPrincipal userNowFromJwt, Guid Id)
    {
        var userId = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value; ;

        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM EnterpriseUser WHERE UserId = {0} AND EnterpriseId = {1}", userId, Id);
    }
    public async Task DeleteOwnership(Guid Id, string userId)
    {
        await _dbContext.Database.ExecuteSqlRawAsync("DELETE FROM EnterpriseUser WHERE UserId = {0} AND EnterpriseId = {1}", userId, Id);
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

    private async Task<EnterpriseDTO> ConvertModelToDTO(EnterpriseModel enterprise)
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

    private async Task<bool> CheckExistTaxAndGLN(string taxCode, string glnCode)
    {
        return await _dbContext.Enterprises.AnyAsync(e => e.TaxCode == taxCode || e.GLNCode == glnCode);
    }
}