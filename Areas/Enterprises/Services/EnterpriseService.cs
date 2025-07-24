using System.Security.Claims;
using System.Threading.Tasks;
using App.Areas.Auth;
using App.Areas.Enterprises.DTO;
using App.Areas.Enterprises.Models;
using Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Enterprises.Services;

public class EnterpriseService : IEnterpriseService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly AppDBContext _dbContext;

    public EnterpriseService(UserManager<AppUser> userManager, AppDBContext dbContext)
    {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    public async Task<(int totalEnterprises, List<EnterpriseDTO> listEnterpriseDTOs)> GetMany(int pageNumber, int limit, string search)
    {
        IQueryable<EnterpriseModel> queryEnterprises = _dbContext.Enterprises;

        if (pageNumber > 0 && limit > 0)
        {
            queryEnterprises = queryEnterprises.Skip((pageNumber - 1) * limit).Take(limit);
        }

        search = search.Trim();

        if (!string.IsNullOrEmpty(search))
        {
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

        IQueryable<EnterpriseModel> queryEnterprises = _dbContext.EnterpriseUsers.Where(eu => eu.UserId == userNow.Id).Select(eu => eu.Enterprise);

        if (pageNumber > 0 && limit > 0)
        {
            queryEnterprises = queryEnterprises.Skip((pageNumber - 1) * limit).Take(limit);
        }

        search = search.Trim();

        if (!string.IsNullOrEmpty(search))
        {
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
        var userNow = await _userManager.GetUserAsync(userNowFromJwt);

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
            UserId = userNow.Id,
            CreatedBy = true
        };

        await _dbContext.EnterpriseUsers.AddAsync(enterpriseUser);
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(ClaimsPrincipal userNowFromJwt, Guid Id)
    {
        var userNow = await _userManager.GetUserAsync(userNowFromJwt);

        string roleUserNow = (await _userManager.GetRolesAsync(userNow))[0];

        var enterprise = await _dbContext.Enterprises.Where(e => e.Id == Id).Include(e => e.EnterpriseUsers).FirstOrDefaultAsync();

        if (roleUserNow != Roles.ADMIN)
        {
            bool isOwner = enterprise.EnterpriseUsers.Any(eu => eu.UserId == userNow.Id);
            if (!isOwner)
            {
                throw new Exception("Không sở hữu doanh nghiệp nên không thể xóa");
            }

            if (enterprise.EnterpriseUsers.Count > 1)
            {
                throw new Exception("Đang sở hữu doanh nghiệp cùng người khác nên không thể xóa");
            }
        }
        
        _dbContext.Enterprises.Remove(enterprise);
        int result = await _dbContext.SaveChangesAsync();

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Xóa doanh nghiệp thất bại");
        }
    }

    public async Task Update(ClaimsPrincipal userNowFromJwt, Guid Id, EnterpriseDTO enterpriseDTO)
    {
        var userNow = await _userManager.GetUserAsync(userNowFromJwt);

        string roleUserNow = (await _userManager.GetRolesAsync(userNow))[0];

        var enterprise = await _dbContext.Enterprises.Where(e => e.Id == Id).Include(e => e.EnterpriseUsers).ThenInclude(eu => eu.User).FirstOrDefaultAsync();

        if (roleUserNow != Roles.ADMIN)
        {
            bool isOwner = enterprise.EnterpriseUsers.Any(eu => eu.UserId == userNow.Id);
            if (!isOwner)
            {
                throw new Exception("Không sở hữu doanh nghiệp nên không thể cập nhật");
            }
        }

        enterprise = ConvertDTOToModel(enterpriseDTO, enterprise);

        _dbContext.Enterprises.Update(enterprise);
        int result = await _dbContext.SaveChangesAsync();

        if (result == 0)
        {
             throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật doanh nghiệp thất bại");
        }
    }

    public Task AddOwnerShip(ClaimsPrincipal userNowFromJwt, Guid Id)
    {
        return null;
    }
    public Task GiveUpOwnership(ClaimsPrincipal userNowFromJwt, Guid Id)
    {
        return null;
    }
    public Task DeleteOwnership(ClaimsPrincipal userNowFromJwt, Guid Id) {
        return null;
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
            Name = enterprise.TaxCode,
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