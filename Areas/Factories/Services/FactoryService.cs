using System.Security.Claims;
using App.Areas.Auth.Mapper;
using App.Areas.Factories.Mapper;
using App.Areas.Factories.DTO;
using App.Areas.Factories.Models;
using App.Areas.Factories.Repositories;
using App.Areas.Enterprises.Mapper;
using App.Areas.Enterprises.Repositories;
using App.Areas.Auth.AuthorizationType;
using System.ComponentModel;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using App.Database;
using Microsoft.AspNetCore.Authorization;
using App.Areas.Factories.Authorization;

namespace App.Areas.Factories.Services;

public class FactoryService : IFactoryService
{
    private readonly IFactoryRepository _factoryRepo;
    private readonly IEnterpriseRepository _enterpriseRepo;
    private readonly IAuthorizationService _authorizationService;

    public FactoryService(IFactoryRepository factoryRepo, IEnterpriseRepository enterpriseRepo, IAuthorizationService authorizationService)
    {
        _factoryRepo = factoryRepo;
        _enterpriseRepo = enterpriseRepo;
        _authorizationService = authorizationService;
    }

    public async Task<(int totalItems, List<FactoryDTO> listDTOs)> GetManyAsync(int pageNumber, int limit, string search)
    {
        int totalFactories = await _factoryRepo.GetTotalAsync();

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<FactoryModel> listFactories = await _factoryRepo.GetManyAsync(pageNumber, limit, search);

        List<FactoryDTO> listFactoryDTOs = new List<FactoryDTO>();

        foreach (var factory in listFactories)
        {
            var factoryDTO = FactoryMapper.ModelToDto(factory);
            AddRelationToDTO(factoryDTO, factory);
            listFactoryDTOs.Add(factoryDTO);
        }

        return (totalFactories, listFactoryDTOs);
    }

    public async Task<(int totalItems, List<FactoryDTO> listDTOs)> GetMyManyAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        int totalMyFactories = await _factoryRepo.GetMyTotalAsync(userIdNow);

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<FactoryModel> listFactories = await _factoryRepo.GetMyManyAsync(userIdNow, pageNumber, limit, search);

        List<FactoryDTO> listFactoryDTOs = new List<FactoryDTO>();

        foreach (var factory in listFactories)
        {
            var factoryDTO = FactoryMapper.ModelToDto(factory);
            AddRelationToDTO(factoryDTO, factory);
            listFactoryDTOs.Add(factoryDTO);
        }

        return (totalMyFactories, listFactoryDTOs);
    }

    public async Task<FactoryDTO> GetOneAsync(Guid id)
    {
        var factory = await _factoryRepo.GetOneAsync(id);
        if (factory == null)
        {
            throw new Exception("Không tìm thấy nhà máy");
        }

        var factoryDTO = FactoryMapper.ModelToDto(factory);
        AddRelationToDTO(factoryDTO, factory);
        return factoryDTO;
    }

    public async Task CreateAsync(FactoryDTO factoryDTO, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var factory = FactoryMapper.DtoToModel(factoryDTO);
        factory.CreatedUserId = userIdNow;
        factory.CreatedAt = DateTime.Now;
        factory.OwnerUserId = userIdNow;

        int result = await _factoryRepo.CreateAsync(factory);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Tạo nhà máy thất bại");
        }
    }

    public async Task UpdateAsync(Guid id, FactoryDTO factoryDTO, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var factory = await _factoryRepo.GetOneAsync(id);

        if (factory == null)
        {
            throw new Exception("Nhà máy không tồn tại");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, factory, new CanUpdateFactoryRequirement());

        if (checkAuth.Succeeded)
        {
            factory = FactoryMapper.DtoToModel(factoryDTO, factory);

            int result = await _factoryRepo.UpdateAsync(factory);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật nhà máy thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền cập nhật nhà máy này");
        }

        
    }

    public async Task AddEnterpriseToFactoryAsync(Guid id, Guid enterpriseId, ClaimsPrincipal userNowFromJwt)
    {
        var existEnterprise = await _enterpriseRepo.CheckExistByIdAsync(enterpriseId);

        if (!existEnterprise)
        {
            throw new Exception("Không tồn tại doanh nghiệp");
        }

        var factory = await _factoryRepo.GetOneAsync(id);

        if (factory == null)
        {
            throw new Exception("Không tồn tại nhà máy");
        }

        if (factory.EnterpriseId == enterpriseId)
        {
            throw new Exception("Nhà máy đã thuộc về doanh nghiệp này rồi");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, factory, new CanAddEnterpriseInFactoryRequirement(enterpriseId));

        if (checkAuth.Succeeded)
        {
            factory.EnterpriseId = enterpriseId;
            factory.OwnerUserId = null;
            int result = await _factoryRepo.UpdateAsync(factory);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Thêm doanh nghiệp sở hữu nhà máy thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền thêm doanh nghiệp vào nhà máy này");
        }
    }

    public async Task DeleteEnterpriseInFactoryAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var factory = await _factoryRepo.GetOneAsync(id);

        if (factory == null)
        {
            throw new Exception("Không tồn tại nhà máy");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, factory, new CanDeleteEnterpriseInFactoryRequirement());

        if (checkAuth.Succeeded)
        {
            factory.EnterpriseId = null;
            int result = await _factoryRepo.UpdateAsync(factory);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa doanh nghiệp sở hữu nhà máy thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa doanh nghiệp của nhà máy này");
        }
    }

    public async Task AddOwnerShipToFactoryAsync(Guid id, string userId, ClaimsPrincipal userNowFromJwt)
    {
        var factory = await _factoryRepo.GetOneAsync(id);

        if (factory == null)
        {
            throw new Exception("Không tồn tại nhà máy");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, factory, new CanAddOwnerToFactoryRequirement(userId));

        if (checkAuth.Succeeded)
        {
            factory.OwnerUserId = userId;
            factory.EnterpriseId = null;

            int result = await _factoryRepo.UpdateAsync(factory);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật cá nhân sở hữu nhà máy thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền cập nhật cá nhân sở hữu nhà máy này");
        }
    }

    public async Task DeleteOwnerShipAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var factory = await _factoryRepo.GetOneAsync(id);

        if (factory == null)
        {
            throw new Exception("Không tồn tại nhà máy");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, factory, new CanDeleteOwnerInFactoryRequirement());

        if (checkAuth.Succeeded)
        {
            factory.OwnerUserId = null;

            int result = await _factoryRepo.UpdateAsync(factory);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa sở hữu cá nhân nhà máy thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa cá nhân sở hữu nhà máy này");
        }
    }

    public async Task DeleteAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var factory = await _factoryRepo.GetOneAsync(id);

        if (factory == null)
        {
            throw new Exception("Không tồn tại nhà máy");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, factory, new CanDeleteFactoryRequirement());

        if (checkAuth.Succeeded)
        {
            int result = await _factoryRepo.DeleteAsync(factory);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa nhà máy thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa nhà máy này");
        }
    }


    private void AddRelationToDTO(FactoryDTO factoryDTO, FactoryModel factory)
    {
        if (factory.CreatedUser != null)
        {
            factoryDTO.CreatedUser = UserMapper.ModelToDto(factory.CreatedUser);
        }

        if (factory.OwnerUser != null)
        {
            factoryDTO.OwnerUser = UserMapper.ModelToDto(factory.OwnerUser);
        }

        if (factory.Enterprise != null)
        {
            factoryDTO.Enterprise = EnterpriseMapper.ModelToDto(factory.Enterprise);
        }
    }
}

/*
Nhà máy chỉ có thể có EnterpriseId hoặc OwnerUserId hoặc không có cả 2 (chỉ có thể có 1 trong 2 loại sở hữu là sở hữu doanh nghiệp và sở hữu cá nhân hoặc không có sở hữu nào, không thể có cả 2)

Logic phân quyền thêm/đổi doanh nghiệp cho nhà máy với role Enterprise: 
- Là chủ cá nhân của nhà máy thì được thêm doanh nghiệp của mình vào nhà máy (không được thêm doanh nghiệp không phải của mình vào nhà máy vì làm vậy là thao tác với tài nguyên không phải của mình).
- Là chủ duy nhất của doanh nghiệp sở hữu nhà máy thì được đổi doanh nghiệp khác của mình vào nhà máy vì tình huống này người đùng là chủ duy nhất của nhà máy.

Logic phân quyền xóa doanh nghiệp sở hữu nhà máy với role Enterprise:
- Là chủ duy nhất của doanh nghiệp sở hữu nhà máy thì được xóa doanh nghiệp sở hữu nhà máy vì tình huống này người đùng là chủ duy nhất của nhà máy.


Logic phân quyền thêm quyền sở hữu cá nhân nhà máy: 
- Là chủ duy nhất của doanh nghiệp sở hữu nhà máy thì được quyền thêm sở hữu cá nhân của bản thân vào nhà máy vì tình huống này người dùng đã là chủ duy nhất của nhà máy.
- Là chủ cá nhân của nhà máy thì không được đổi chủ vì đổi như vậy là thao tác trực tiếp với dữ liệu của User khác => Không có quyền.

Logic phân quyền xóa sở hữu cá nhân của nhà máy:
- Là chủ cá nhân của nhà máy thì có quyền xóa sở hữu cá nhân của nhà máy.

Logic phân quyền xóa nhà máy:
- Là chủ cá nhân của nhà máy thì có quyền xóa nhà máy.
- Là chủ duy nhất của doanh nghiệp sở hữu nhà máy thì có quyền xóa nhà máy vì tình huống này người dùng đã là chủ duy nhất của nhà máy.
*/