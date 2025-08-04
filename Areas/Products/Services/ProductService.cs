using System.Security.Claims;
using App.Areas.Auth.AuthorizationType;
using App.Areas.Auth.Mapper;
using App.Areas.Categories.Mapper;
using App.Areas.Enterprises.Mapper;
using App.Areas.Enterprises.Repositories;
using App.Areas.Factories.Mapper;
using App.Areas.Factories.Repositories;
using App.Areas.IndividualEnterprises.Mapper;
using App.Areas.IndividualEnterprises.Repositories;
using App.Areas.Products.Authorization;
using App.Areas.Products.DTO;
using App.Areas.Products.Mapper;
using App.Areas.Products.Models;
using App.Areas.Products.Repositories;
using App.Database;
using App.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace App.Areas.Products.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepo;
    private readonly IAuthorizationService _authorizationService;
    private readonly IIndividualEnterpiseRepository _individualEnterpriseRepo;
    private readonly IEnterpriseRepository _enterpriseRepo;
    private readonly IFactoryRepository _factoryRepo;
    private readonly UserManager<AppUser> _userManager;

    public ProductService(IProductRepository productRepo, IAuthorizationService authorizationService, IIndividualEnterpiseRepository individualEnterpiseRepo, IEnterpriseRepository enterpriseRepo, IFactoryRepository factoryRepo, UserManager<AppUser> userManager)
    {
        _productRepo = productRepo;
        _authorizationService = authorizationService;
        _individualEnterpriseRepo = individualEnterpiseRepo;
        _enterpriseRepo = enterpriseRepo;
        _factoryRepo = factoryRepo;
        _userManager = userManager;
    }

    public async Task<(int totalItems, List<ProductDTO> listDTOs)> GetManyAsync(int pageNumber, int limit, string search)
    {
        int totalProducts = await _productRepo.GetTotalAsync();

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<ProductModel> listProducts = await _productRepo.GetManyAsync(pageNumber, limit, search);
        List<ProductDTO> listProductDtos = new List<ProductDTO>();

        foreach (var product in listProducts)
        {
            var productDTO = ProductMapper.ModelToDto(product);
            AddRelationToDTO(productDTO, product);
            listProductDtos.Add(productDTO);
        }

        return (totalProducts, listProductDtos);
    }

    public async Task<(int totalItems, List<ProductDTO> listDTOs)> GetMyManyAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        int totalProducts = await _productRepo.GetMyTotalAsync(userIdNow);

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<ProductModel> listProducts = await _productRepo.GetMyManyAsync(userIdNow, pageNumber, limit, search);
        List<ProductDTO> listProductDtos = new List<ProductDTO>();

        foreach (var product in listProducts)
        {
            var productDTO = ProductMapper.ModelToDto(product);
            AddRelationToDTO(productDTO, product);
            listProductDtos.Add(productDTO);
        }

        return (totalProducts, listProductDtos);
    }

    public async Task<ProductDTO> GetOneByIdAsync(Guid id)
    {
        var product = await _productRepo.GetOneByIdAsync(id);
        if (product == null)
        {
            throw new Exception("Không tìm thấy sản phẩm");
        }
        var productDTO = ProductMapper.ModelToDto(product);
        AddRelationToDTO(productDTO, product);

        return productDTO;
    }

    public async Task<ProductDTO> GetOneByTraceCodeAsync(string traceCode)
    {
        var product = await _productRepo.GetOneByTraceCodeAsync(traceCode);
        if (product == null)
        {
            throw new Exception("Không tìm thấy sản phẩm");
        }
        var productDTO = ProductMapper.ModelToDto(product);
        AddRelationToDTO(productDTO, product);

        return productDTO;
    }

    public async Task<(int totalProducts, List<ProductDTO> productDTOs)> GetManyByOwnerIndividualEnterpriseAsync(string individualEnterpriseId, int pageNumber, int limit, string search)
    {
        int totalProducts = await _productRepo.GetTotalByOwnerIndividualEnterpriseAsync(individualEnterpriseId);

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<ProductModel> listProducts = await _productRepo.GetManyByOwnerIndividualEnterpriseAsync(individualEnterpriseId, pageNumber, limit, search);
        List<ProductDTO> listProductDtos = new List<ProductDTO>();

        foreach (var product in listProducts)
        {
            var productDTO = ProductMapper.ModelToDto(product);
            AddRelationToDTO(productDTO, product);
            listProductDtos.Add(productDTO);
        }

        return (totalProducts, listProductDtos);
    }

    public async Task<(int totalProducts, List<ProductDTO> productDTOs)> GetManyByOwnerEnterpriseAsync(Guid enterpriseId, int pageNumber, int limit, string search)
    {
        int totalProducts = await _productRepo.GetTotalByOwnerEnterpriseAsync(enterpriseId);

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<ProductModel> listProducts = await _productRepo.GetManyByOwnerEnterpriseAsync(enterpriseId, pageNumber, limit, search);
        List<ProductDTO> listProductDtos = new List<ProductDTO>();

        foreach (var product in listProducts)
        {
            var productDTO = ProductMapper.ModelToDto(product);
            AddRelationToDTO(productDTO, product);
            listProductDtos.Add(productDTO);
        }

        return (totalProducts, listProductDtos);
    }

    public async Task<(int totalProducts, List<ProductDTO> productDTOs)> GetManyByCarrierEnterpriseAsync(Guid enterpriseId, int pageNumber, int limit, string search)
    {
        int totalProducts = await _productRepo.GetTotalByCarrierEnterpriseAsync(enterpriseId);

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<ProductModel> listProducts = await _productRepo.GetManyByCarrierEnterpriseAsync(enterpriseId, pageNumber, limit, search);
        List<ProductDTO> listProductDtos = new List<ProductDTO>();

        foreach (var product in listProducts)
        {
            var productDTO = ProductMapper.ModelToDto(product);
            AddRelationToDTO(productDTO, product);
            listProductDtos.Add(productDTO);
        }

        return (totalProducts, listProductDtos);
    }

    public async Task<(int totalProducts, List<ProductDTO> productDTOs)> GetManyByCategoryAsync(Guid categoryId, int pageNumber, int limit, string search)
    {
        int totalProducts = await _productRepo.GetTotalByCategoryAsync(categoryId);

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<ProductModel> listProducts = await _productRepo.GetManyByCategoryAsync(categoryId, pageNumber, limit, search);
        List<ProductDTO> listProductDtos = new List<ProductDTO>();

        foreach (var product in listProducts)
        {
            var productDTO = ProductMapper.ModelToDto(product);
            AddRelationToDTO(productDTO, product);
            listProductDtos.Add(productDTO);
        }

        return (totalProducts, listProductDtos);
    }

    public async Task<(int totalProducts, List<ProductDTO> productDTOs)> GetManyByProducerEnterpriseAsync(Guid enterpriseId, int pageNumber, int limit, string search)
    {
        int totalProducts = await _productRepo.GetTotalByProducerEnterpriseAsync(enterpriseId);

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<ProductModel> listProducts = await _productRepo.GetManyByProducerEnterpriseAsync(enterpriseId, pageNumber, limit, search);
        List<ProductDTO> listProductDtos = new List<ProductDTO>();

        foreach (var product in listProducts)
        {
            var productDTO = ProductMapper.ModelToDto(product);
            AddRelationToDTO(productDTO, product);
            listProductDtos.Add(productDTO);
        }

        return (totalProducts, listProductDtos);
    }

    public async Task<(int totalProducts, List<ProductDTO> productDTOs)> GetManyByResponsibleUserAsync(string userId, int pageNumber, int limit, string search)
    {
        int totalProducts = await _productRepo.GetTotalByResponsibleUserAsync(userId);

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<ProductModel> listProducts = await _productRepo.GetManyByResponsibleUserAsync(userId, pageNumber, limit, search);
        List<ProductDTO> listProductDtos = new List<ProductDTO>();

        foreach (var product in listProducts)
        {
            var productDTO = ProductMapper.ModelToDto(product);
            AddRelationToDTO(productDTO, product);
            listProductDtos.Add(productDTO);
        }

        return (totalProducts, listProductDtos);
    }

    public async Task<(int totalProducts, List<ProductDTO> productDTOs)> GetManyByFactoryAsync(Guid factoryId, int pageNumber, int limit, string search)
    {
        int totalProducts = await _productRepo.GetTotalByFactoryAsync(factoryId);

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<ProductModel> listProducts = await _productRepo.GetManyByCategoryAsync(factoryId, pageNumber, limit, search);
        List<ProductDTO> listProductDtos = new List<ProductDTO>();

        foreach (var product in listProducts)
        {
            var productDTO = ProductMapper.ModelToDto(product);
            AddRelationToDTO(productDTO, product);
            listProductDtos.Add(productDTO);
        }

        return (totalProducts, listProductDtos);
    }

    public async Task CreateAsync(ProductDTO productDTO, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (productDTO.OwnerIsIndividualEnterprise && productDTO.OwnerEnterpriseId != null)
        {
            throw new Exception("Không thể tạo sản phẩm vừa của hộ kinh doanh cá nhân, vừa của doanh nghiệp");
        }
        else if (!productDTO.OwnerIsIndividualEnterprise && productDTO.OwnerEnterpriseId == null)
        {
            throw new Exception("Không thể tạo sản phẩm không có chủ sở hữu");
        }

        string traceCode = "";
        if (productDTO.TraceCode != null)
        {
            bool existTraceCode = await _productRepo.CheckExistByTraceCode(PrefixCode.PRODUCT + productDTO.TraceCode);

            if (existTraceCode)
            {
                throw new Exception("Mã sản phẩm đã tồn tại nên không tạo sản phẩm");
            }

            traceCode = PrefixCode.PRODUCT + productDTO.TraceCode;
        }
        else
        {
            traceCode = CreateCode.GenerateCodeFromTicks(PrefixCode.PRODUCT);
        }

        if (productDTO.OwnerIsIndividualEnterprise)
        {
            bool isOwnerIndividualEnterprise = await _individualEnterpriseRepo.CheckExistByOwnerUserIdAsync(userIdNow);

            if (!isOwnerIndividualEnterprise)
            {
                throw new Exception("Không sở hữu hộ kinh doanh cá nhân nên không thể tạo sản phẩm cho hộ kinh doanh cá nhân");
            }
        }
        else if (productDTO.OwnerEnterpriseId != null)
        {
            var enterpriseId = (Guid)productDTO.OwnerEnterpriseId;

            bool isOwnerEnterprise = await _enterpriseRepo.CheckIsOwner(enterpriseId, userIdNow);

            if (!isOwnerEnterprise)
            {
                throw new Exception("Không sở hữu doanh nghiệp nên không thể tạo sản phẩm cho hộ kinh doanh cá nhân");
            }
        }

        var product = ProductMapper.DtoToModel(productDTO);
        product.CreatedUserId = userIdNow;
        product.CreatedAt = DateTime.Now;
        product.TraceCode = traceCode;
        if (productDTO.OwnerIsIndividualEnterprise)
        {
            product.OwnerIndividualEnterpriseId = userIdNow;
        }
        else
        {
            product.OwnerEnterpriseId = productDTO.OwnerEnterpriseId;
        }

        int result = await _productRepo.CreateAsync(product);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Tạo sản phẩm thất bại");
        }
        
    }

    public async Task UpdateAsync(Guid id, ProductDTO productDTO, ClaimsPrincipal userNowFromJwt)
    {
        var product = await _productRepo.GetOneByIdAsync(id);

        if (product == null)
        {
            throw new Exception("sản phẩm không tồn tại");
        }

        string traceCode = product.TraceCode;
        if (productDTO.TraceCode != null)
        {
            bool existtraceCode = await _productRepo.CheckExistExceptThisByTraceCode(id, PrefixCode.PRODUCT + productDTO.TraceCode);

            if (existtraceCode)
            {
                throw new Exception("Mã sản phẩm đã tồn tại nên không cập nhật sản phẩm");
            }

            traceCode = PrefixCode.PRODUCT + productDTO.TraceCode;
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, product, new CanUpdateProductRequirement(traceCode));

        if (checkAuth.Succeeded)
        {
            product = ProductMapper.DtoToModel(productDTO, product);
            product.TraceCode = traceCode;
            product.UpdatedAt = DateTime.Now;
            product.UpdatedUserId = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int result = await _productRepo.UpdateAsync(product);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền cập nhật sản phẩm này");
        }
    }

    public async Task DeleteAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var product = await _productRepo.GetOneByIdAsync(id);

        if (product == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, product, new CanDeleteProductRequirement());

        if (checkAuth.Succeeded)
        {
            int result = await _productRepo.DeleteAsync(product);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa sản phẩm này");
        }
    }

    public async Task AddOwnerIndividualEnterpriseOfProductAsync(Guid id, string individualEnterpriseId, ClaimsPrincipal userNowFromJwt)
    {
        bool existIndividualEnterprise = await _individualEnterpriseRepo.CheckExistByOwnerUserIdAsync(individualEnterpriseId);

        if (!existIndividualEnterprise)
        {
            throw new Exception("Không tồn tại hộ kinh doanh cá nhân");
        }

        var product = await _productRepo.GetOneByIdAsync(id);

        if (product == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, product, new CanAddOwnerIndividualEnterpriseOfProductRequirement(individualEnterpriseId));

        if (checkAuth.Succeeded)
        {
            product.OwnerIndividualEnterpriseId = individualEnterpriseId;
            product.OwnerEnterpriseId = null;
            product.UpdatedUserId = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int result = await _productRepo.UpdateAsync(product);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Thêm hộ kinh doanh của sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền thêm hộ kinh doanh cá nhân vào sản phẩm này");
        }
    }

    public async Task DeleteOwnerIndividualEnterpriseOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var product = await _productRepo.GetOneByIdAsync(id);

        if (product == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, product, new CanDeleteOwnerIndividualEnterpriseOfProductRequirement());

        if (checkAuth.Succeeded)
        {
            product.OwnerIndividualEnterpriseId = null;
            product.UpdatedUserId = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            int result = await _productRepo.UpdateAsync(product);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa sở hữu cá nhân sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa cá nhân sở hữu sản phẩm này");
        }
    }

    public async Task AddOwnerEnterpriseOfProductAsync(Guid id, Guid enterpriseId, ClaimsPrincipal userNowFromJwt)
    {
        var existEnterprise = await _enterpriseRepo.CheckExistByIdAsync(enterpriseId);

        if (!existEnterprise)
        {
            throw new Exception("Không tồn tại doanh nghiệp");
        }

        var product = await _productRepo.GetOneByIdAsync(id);

        if (product == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        if (product.OwnerEnterpriseId == enterpriseId)
        {
            throw new Exception("sản phẩm đã thuộc về doanh nghiệp này rồi");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, product, new CanAddOwnerEnterpriseOfProductRequirement(enterpriseId));

        if (checkAuth.Succeeded)
        {
            product.OwnerEnterpriseId = enterpriseId;
            product.OwnerIndividualEnterpriseId = null;
            product.UpdatedUserId = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int result = await _productRepo.UpdateAsync(product);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Thêm doanh nghiệp sở hữu sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền thêm doanh nghiệp vào sản phẩm này");
        }
    }

    public async Task DeleteOwnerEnterpriseOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var product = await _productRepo.GetOneByIdAsync(id);

        if (product == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, product, new CanDeleteEnterpriseInFactoryRequirement());

        if (checkAuth.Succeeded)
        {
            product.OwnerEnterpriseId = null;
            product.UpdatedUserId = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int result = await _productRepo.UpdateAsync(product);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa doanh nghiệp sở hữu sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa doanh nghiệp của sản phẩm này");
        }
    }

    public async Task AddCarrierEnterpriseOfProductAsync(Guid id, Guid enterpriseId, ClaimsPrincipal userNowFromJwt)
    {
        var existEnterprise = await _enterpriseRepo.CheckExistByIdAsync(enterpriseId);

        if (!existEnterprise)
        {
            throw new Exception("Không tồn tại doanh nghiệp");
        }

        var product = await _productRepo.GetOneByIdAsync(id);

        if (product == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, product, new CanUpdateSomeRelationsProductRequirement());

        if (checkAuth.Succeeded)
        {
            product.CarrierEnterpriseId = enterpriseId;
            product.UpdatedUserId = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int result = await _productRepo.UpdateAsync(product);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật doanh nghiệp vận chuyển sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền cập nhật doanh nghiệp vận chuyển của sản phẩm này");
        }
    }

    public async Task DeleteCarrierEnterpriseOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var product = await _productRepo.GetOneByIdAsync(id);

        if (product == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, product, new CanUpdateSomeRelationsProductRequirement());

        if (checkAuth.Succeeded)
        {
            product.CarrierEnterpriseId = null;
            product.UpdatedUserId = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int result = await _productRepo.UpdateAsync(product);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa doanh nghiệp vận chuyển sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa doanh nghiệp vận chuyển của sản phẩm này");
        }
    }

    public async Task AddProducerEnterpriseOfProductAsync(Guid id, Guid enterpriseId, ClaimsPrincipal userNowFromJwt)
    {
        var existEnterprise = await _enterpriseRepo.CheckExistByIdAsync(enterpriseId);

        if (!existEnterprise)
        {
            throw new Exception("Không tồn tại doanh nghiệp");
        }

        var product = await _productRepo.GetOneByIdAsync(id);

        if (product == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, product, new CanUpdateSomeRelationsProductRequirement());

        if (checkAuth.Succeeded)
        {
            product.ProducerEnterpriseId = enterpriseId;
            product.UpdatedUserId = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int result = await _productRepo.UpdateAsync(product);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật doanh nghiệp sản xuất sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền cập nhật doanh nghiệp sản xuất của sản phẩm này");
        }
    }

    public async Task DeleteProducerEnterpriseOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var product = await _productRepo.GetOneByIdAsync(id);

        if (product == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, product, new CanUpdateSomeRelationsProductRequirement());

        if (checkAuth.Succeeded)
        {
            product.ProducerEnterpriseId = null;
            product.UpdatedUserId = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int result = await _productRepo.UpdateAsync(product);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa doanh nghiệp sản xuất sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa doanh nghiệp sản xuất của sản phẩm này");
        }
    }

    public async Task AddResponsibleUserOfProductAsync(Guid id, string userId, ClaimsPrincipal userNowFromJwt)
    {
        var responsibleUser = await _userManager.FindByIdAsync(userId);

        if (responsibleUser == null)
        {
            throw new Exception("Không tồn tại người dùng");
        }

        var roleResponsibleUser = (await _userManager.GetRolesAsync(responsibleUser))[0];

        if (roleResponsibleUser != Roles.ADMIN && roleResponsibleUser != Roles.ENTERPRISE)
        {
            throw new Exception("Người dùng này không có vai trò phù hợp làm người phụ trách sản phẩm");
        }

        var product = await _productRepo.GetOneByIdAsync(id);

        if (product == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, product, new CanUpdateSomeRelationsProductRequirement());

        if (checkAuth.Succeeded)
        {
            product.ResponsibleUserId = userId;
            product.UpdatedUserId = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int result = await _productRepo.UpdateAsync(product);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật người phụ trách sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền cập nhật người phụ trách sản phẩm này");
        }
    }

    public async Task DeleteResponsibleUserOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var product = await _productRepo.GetOneByIdAsync(id);

        if (product == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, product, new CanUpdateSomeRelationsProductRequirement());

        if (checkAuth.Succeeded)
        {
            product.ResponsibleUserId = null;
            product.UpdatedUserId = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int result = await _productRepo.UpdateAsync(product);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa người phụ trách sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa người phụ trách sản phẩm này");
        }
    }

    public async Task AddFactoryOfProductAsync(Guid id, Guid factoryId, ClaimsPrincipal userNowFromJwt)
    {
         var existFactory = await _factoryRepo.CheckExistByIdAsync(factoryId);

        if (!existFactory)
        {
            throw new Exception("Không tồn tại nhà máy");
        }

        var product = await _productRepo.GetOneByIdAsync(id);

        if (product == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, product, new CanUpdateSomeRelationsProductRequirement());

        if (checkAuth.Succeeded)
        {
            product.FactoryId = factoryId;
            product.UpdatedUserId = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int result = await _productRepo.UpdateAsync(product);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật nhà máy của sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền cập nhật nhà máy của sản phẩm này");
        }
    }

    public async Task DeleteFactoryOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var product = await _productRepo.GetOneByIdAsync(id);

        if (product == null)
        {
            throw new Exception("Không tồn tại sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, product, new CanUpdateSomeRelationsProductRequirement());

        if (checkAuth.Succeeded)
        {
            product.FactoryId = null;
            product.UpdatedUserId = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int result = await _productRepo.UpdateAsync(product);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa nhà máy của sản phẩm thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa nhà máy của sản phẩm này");
        }
    }

    private void AddRelationToDTO(ProductDTO productDTO, ProductModel product)
    {
        if (product.OwnerIndividualEnterprise != null)
        {
            productDTO.OwnerIndividualEnterprise = IndividualEnterpriseMapper.ModelToDto(product.OwnerIndividualEnterprise);
        }

        if (product.OwnerEnterprise != null)
        {
            productDTO.OwnerEnterprise = EnterpriseMapper.ModelToDto(product.OwnerEnterprise);
        }

        if (product.Category != null)
        {
            productDTO.Category = CategoryMapper.ModelToDto(product.Category);
        }

        if (product.ProducerEnterprise != null)
        {
            productDTO.ProducerEnterprise = EnterpriseMapper.ModelToDto(product.ProducerEnterprise);
        }

        if (product.CarrierEnterprise != null)
        {
            productDTO.CarrierEnterprise = EnterpriseMapper.ModelToDto(product.CarrierEnterprise);
        }

        if (product.ResponsibleUser != null)
        {
            productDTO.ResponsibleUser = UserMapper.ModelToDto(product.ResponsibleUser);
        }

        if (product.Factory != null)
        {
            productDTO.Factory = FactoryMapper.ModelToDto(product.Factory);
        }

        if (product.CreatedUser != null)
        {
            productDTO.CreatedUser = UserMapper.ModelToDto(product.CreatedUser);
        }

        if (product.UpdatedUser != null)
        {
            productDTO.UpdatedUser = UserMapper.ModelToDto(product.UpdatedUser);
        }
    }
}