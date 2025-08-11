using System.Security.Claims;
using App.Areas.Products.DTO;
using App.Services;

namespace App.Areas.Products.Services;

public interface IProductService : IBaseService<ProductDTO>
{
    public Task<ProductDTO> GetOneByTraceCodeAsync(string traceCode);

    public Task<(int totalProducts, List<ProductDTO> productDTOs)> GetManyByCategoryAsync(Guid categoryId, int pageNumber, int limit, string search, bool descending);

    public Task<(int totalProducts, List<ProductDTO> productDTOs)> GetManyByOwnerIndividualEnterpriseAsync(Guid individualEnterpriseId, int pageNumber, int limit, string search, bool descending);

    public Task<(int totalProducts, List<ProductDTO> productDTOs)> GetManyByOwnerEnterpriseAsync(Guid enterpriseId, int pageNumber, int limit, string search, bool descending);

    public Task<(int totalProducts, List<ProductDTO> productDTOs)> GetManyByCarrierEnterpriseAsync(Guid enterpriseId, int pageNumber, int limit, string search, bool descending);

    public Task<(int totalProducts, List<ProductDTO> productDTOs)> GetManyByProducerEnterpriseAsync(Guid enterpriseId, int pageNumber, int limit, string search, bool descending);

    public Task<(int totalProducts, List<ProductDTO> productDTOs)> GetManyByResponsibleUserAsync(Guid userId, int pageNumber, int limit, string search, bool descending);

    public Task<(int totalProducts, List<ProductDTO> productDTOs)> GetManyByFactoryAsync(Guid factoryId, int pageNumber, int limit, string search, bool descending);

    public Task AddOwnerIndividualEnterpriseOfProductAsync(Guid id, Guid individualEnterpriseId, ClaimsPrincipal userNowFromJwt);

    public Task DeleteOwnerIndividualEnterpriseOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt);

    public Task AddOwnerEnterpriseOfProductAsync(Guid id, Guid enterpriseId, ClaimsPrincipal userNowFromJwt);

    public Task DeleteOwnerEnterpriseOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt);

    public Task AddCarrierEnterpriseOfProductAsync(Guid id, Guid enterpriseId, ClaimsPrincipal userNowFromJwt);

    public Task DeleteCarrierEnterpriseOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt);

    public Task AddProducerEnterpriseOfProductAsync(Guid id, Guid enterpriseId, ClaimsPrincipal userNowFromJwt);

    public Task DeleteProducerEnterpriseOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt);

    public Task AddResponsibleUserOfProductAsync(Guid id, Guid userId, ClaimsPrincipal userNowFromJwt);

    public Task DeleteResponsibleUserOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt);

    public Task AddFactoryOfProductAsync(Guid id, Guid factoryId, ClaimsPrincipal userNowFromJwt);

    public Task DeleteFactoryOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt);

    public Task UploadPhotosOfProductAsync(Guid id, List<IFormFile> listFiles, ClaimsPrincipal userNowFromJwt);

    public Task DeletePhotoOfProductAsync(Guid id, Guid fileId, ClaimsPrincipal userNowFromJwt);
}