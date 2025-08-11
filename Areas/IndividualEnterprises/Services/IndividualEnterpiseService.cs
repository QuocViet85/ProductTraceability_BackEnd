using System.Security.Claims;
using App.Areas.Auth.AuthorizationType;
using App.Areas.Auth.Mapper;
using App.Areas.Enterprises.Repositories;
using App.Areas.IndividualEnterprises.DTO;
using App.Areas.IndividualEnterprises.Mapper;
using App.Areas.IndividualEnterprises.Model;
using App.Areas.IndividualEnterprises.Repositories;
using App.Helper;

namespace App.Areas.IndividualEnterprises.Services;

public class IndividualEnterpiseService : IIndividualEnterpiseService
{
    private readonly IIndividualEnterpiseRepository _individualEnterpiseRepo;

    private readonly IEnterpriseRepository _enterpriseRepo;

    public IndividualEnterpiseService(IIndividualEnterpiseRepository individualEnterpiseRepo, IEnterpriseRepository enterpriseRepo)
    {
        _individualEnterpiseRepo = individualEnterpiseRepo;
        _enterpriseRepo = enterpriseRepo;
    }


    public async Task<(int totalItems, List<IndividualEnterpriseDTO> listDTOs)> GetManyAsync(int pageNumber, int limit, string search, bool descending)
    {
        int totalIndividualEnterpise = await _individualEnterpiseRepo.GetTotalAsync();

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<IndividualEnterpriseModel> listIndividualEnterprises = await _individualEnterpiseRepo.GetManyAsync(pageNumber, limit, search, descending);

        List<IndividualEnterpriseDTO> listIndividualEnterpriseDtos = new List<IndividualEnterpriseDTO>();

        foreach (var individualEnterprise in listIndividualEnterprises)
        {
            var individualEnterpriseDTO = IndividualEnterpriseMapper.ModelToDto(individualEnterprise);
            AddRelationToDto(individualEnterpriseDTO, individualEnterprise);
            listIndividualEnterpriseDtos.Add(individualEnterpriseDTO);
        }

        return (totalIndividualEnterpise, listIndividualEnterpriseDtos);
    }

    public async Task<IndividualEnterpriseDTO> GetOneByIdAsync(Guid id)
    {
        var individualEnterprise = await _individualEnterpiseRepo.GetOneByIdAsync(id);

        if (individualEnterprise == null)
        {
            throw new Exception("Không tìm thấy hộ kinh doanh cá nhân");
        }

        var individualEnterpriseDTO = IndividualEnterpriseMapper.ModelToDto(individualEnterprise);
        AddRelationToDto(individualEnterpriseDTO, individualEnterprise);
        return individualEnterpriseDTO;
    }

    public async Task<IndividualEnterpriseDTO> GetMyOneAsync(ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var individualEnterprise = await _individualEnterpiseRepo.GetMyOneAsync(Guid.Parse(userIdNow));

        if (individualEnterprise == null)
        {
            throw new Exception("Không sở hữu hộ kinh doanh cá nhân");
        }

        var individualEnterpriseDTO = IndividualEnterpriseMapper.ModelToDto(individualEnterprise);
        AddRelationToDto(individualEnterpriseDTO, individualEnterprise);
        return individualEnterpriseDTO;
    }
    public async Task CreateAsync(IndividualEnterpriseDTO individualEnterpriseDTO, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (await _individualEnterpiseRepo.CheckExistByOwnerUserIdAsync(Guid.Parse(userIdNow)));
        {
            throw new UnauthorizedAccessException("Đã sở hữu hộ kinh doanh cá nhân rồi nên không thể tạo thêm hộ kinh doanh cá nhân nữa");
        }

        if (await _individualEnterpiseRepo.CheckExistByTaxCodeAndGLNCodeAsync(individualEnterpriseDTO.TaxCode, individualEnterpriseDTO.GLNCode) || await _enterpriseRepo.CheckExistByCodeAsync(individualEnterpriseDTO.TaxCode, individualEnterpriseDTO.GLNCode))
        {
            throw new Exception("Không thể tạo hộ kinh doanh cá nhân vì mã số thuế hoặc mã GLN đã tồn tại");
        }

        string individualEnterpiseCode = "";
        if (individualEnterpriseDTO.IndividualEnterpriseCode != null)
        {
            bool existIndividualEnterpriseCode = await _individualEnterpiseRepo.CheckExistByIndividualEnterpriseCodeAsync(PrefixCode.INDIVIDUAL_ENTERPRISE + individualEnterpriseDTO.IndividualEnterpriseCode);

            if (existIndividualEnterpriseCode)
            {
                throw new Exception("Mã hộ kinh doanh cá nhân đã tồn tại nên không thể tạo hộ kinh doanh cá nhân");
            }

            individualEnterpiseCode = PrefixCode.INDIVIDUAL_ENTERPRISE + individualEnterpriseDTO.IndividualEnterpriseCode;
        }
        else
        {
            individualEnterpiseCode = CreateCode.GenerateCodeFromTicks(PrefixCode.INDIVIDUAL_ENTERPRISE);
        }

        var individualEnterprise = IndividualEnterpriseMapper.DtoToModel(individualEnterpriseDTO);
        individualEnterprise.OwnerUserId = Guid.Parse(userIdNow);
        individualEnterprise.IndividualEnterpriseCode = individualEnterpiseCode;
        individualEnterprise.CreatedAt = DateTime.Now;

        int result = await _individualEnterpiseRepo.CreateAsync(individualEnterprise);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Tạo hộ kinh doanh cá nhân thất bại");
        }
    }

    public async Task DeleteAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var userId = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var individualEnterpise = await _individualEnterpiseRepo.GetOneByIdAsync(id);

        if (individualEnterpise == null)
        {
            throw new Exception("Không tồn tại hộ kinh doanh cá nhân");
        }

        if (!userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            bool isOwner = userId == individualEnterpise.OwnerUserId.ToString();

            if (!isOwner)
            {
                throw new UnauthorizedAccessException("Không phải chủ sở hữu hộ kinh doanh cá nhân nên không được xóa");
            }
        }

        int result = await _individualEnterpiseRepo.DeleteAsync(individualEnterpise);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Xóa hộ kinh doanh cá nhân thất bại");
        }
    }

    public async Task UpdateAsync(Guid id, IndividualEnterpriseDTO individualEnterpriseDTO, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var individualEnterpise = await _individualEnterpiseRepo.GetOneByIdAsync(id);

        if (individualEnterpise == null)
        {
            throw new Exception("Không tồn tại hộ kinh doanh cá nhân");
        }

        if (!userNowFromJwt.IsInRole(Roles.ADMIN))
        {
            bool isOwner = userIdNow == individualEnterpise.OwnerUserId.ToString();

            if (!isOwner)
            {
                throw new UnauthorizedAccessException("Không phải chủ sở hữu hộ kinh doanh cá nhân nên không được cập nhật");
            }
        }

        if (await _individualEnterpiseRepo.CheckExistExceptThisByTaxCodeAndGLNCodeAsync(id, individualEnterpriseDTO.TaxCode, individualEnterpriseDTO.GLNCode) || await _enterpriseRepo.CheckExistByCodeAsync(individualEnterpriseDTO.TaxCode, individualEnterpriseDTO.GLNCode))
        {
            throw new Exception("Không thể cập nhật hộ kinh doanh cá nhân vì mã số thuế hoặc mã GLN đã tồn tại");
        }

        string individualEnterpiseCode = individualEnterpise.IndividualEnterpriseCode;
        if (individualEnterpriseDTO.IndividualEnterpriseCode != null)
        {
            bool existIndividualEnterpriseCode = await _individualEnterpiseRepo.CheckExistExceptThisByIndividualEnterpriseCodeAsync(id, PrefixCode.INDIVIDUAL_ENTERPRISE + individualEnterpriseDTO.IndividualEnterpriseCode);

            if (existIndividualEnterpriseCode)
            {
                throw new Exception("Mã hộ kinh doanh cá nhân đã tồn tại nên không thể cập nhật hộ kinh doanh cá nhân");
            }

            individualEnterpiseCode = PrefixCode.INDIVIDUAL_ENTERPRISE + individualEnterpriseDTO.IndividualEnterpriseCode;
        }

        individualEnterpise = IndividualEnterpriseMapper.DtoToModel(individualEnterpriseDTO, individualEnterpise);
        individualEnterpise.IndividualEnterpriseCode = individualEnterpiseCode;
        individualEnterpise.UpdatedAt = DateTime.Now;
        individualEnterpise.UpdatedUserId = Guid.Parse(userIdNow);

        int result = await _individualEnterpiseRepo.UpdateAsync(individualEnterpise);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật hộ kinh doanh cá nhân thất bại");
        }
    }

    public async Task<IndividualEnterpriseDTO> GetOneByIndividualEnterpriseCodeAsync(string individualEnterpiseCode)
    {
        var individualEnterprise = await _individualEnterpiseRepo.GetOneByIndividualEnterpriseCodeAsync(individualEnterpiseCode);

        if (individualEnterprise == null)
        {
            throw new Exception("Không tìm thấy hộ kinh doanh cá nhân");
        }

        var individualEnterpriseDTO = IndividualEnterpriseMapper.ModelToDto(individualEnterprise);
        AddRelationToDto(individualEnterpriseDTO, individualEnterprise);
        return individualEnterpriseDTO;
    }

    private void AddRelationToDto(IndividualEnterpriseDTO individualEnterpriseDTO, IndividualEnterpriseModel individualEnterprise)
    {
        if (individualEnterprise.OwnerUser != null)
        {
            individualEnterpriseDTO.OwnerUser = UserMapper.ModelToDto(individualEnterprise.OwnerUser);
        }

        if (individualEnterprise.UpdatedUser != null)
        {
            individualEnterpriseDTO.UpdatedUser = UserMapper.ModelToDto(individualEnterprise.UpdatedUser);
        }
    }
    //

    public Task<(int totalItems, List<IndividualEnterpriseDTO> listDTOs)> GetMyManyAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }
}