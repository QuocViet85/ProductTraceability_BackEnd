using System.Security.Claims;
using App.Areas.Products.DTO;
using App.Services;

namespace App.Areas.Products.Services;

public interface IProductService : IBaseService<ProductDTO>
{
    public Task<ProductDTO> GetOneByTraceCodeAsync(string traceCode);

    public Task<(int totalProducts, List<ProductDTO> productDTOs)> GetManyByCategoryAsync(Guid categoryId, int pageNumber, int limit, string search);

    public Task<(int totalProducts, List<ProductDTO> productDTOs)> GetManyByOwnerIndividualEnterpriseAsync(string individualEnterpriseId, int pageNumber, int limit, string search);

    public Task<(int totalProducts, List<ProductDTO> productDTOs)> GetManyByOwnerEnterpriseAsync(Guid enterpriseId, int pageNumber, int limit, string search);

    public Task<(int totalProducts, List<ProductDTO> productDTOs)> GetManyByCarrierEnterpriseAsync(Guid enterpriseId, int pageNumber, int limit, string search);

    public Task<(int totalProducts, List<ProductDTO> productDTOs)> GetManyByProducerEnterpriseAsync(Guid enterpriseId, int pageNumber, int limit, string search);

    public Task<(int totalProducts, List<ProductDTO> productDTOs)> GetManyByResponsibleUserAsync(string userId, int pageNumber, int limit, string search);

    public Task<(int totalProducts, List<ProductDTO> productDTOs)> GetManyByFactoryAsync(Guid factoryId, int pageNumber, int limit, string search);

    public Task AddOwnerIndividualEnterpriseOfProductAsync(Guid id, string individualEnterpriseId, ClaimsPrincipal userNowFromJwt);

    public Task DeleteOwnerIndividualEnterpriseOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt);

    public Task AddOwnerEnterpriseOfProductAsync(Guid id, Guid enterpriseId, ClaimsPrincipal userNowFromJwt);

    public Task DeleteOwnerEnterpriseOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt);

    public Task AddCarrierEnterpriseOfProductAsync(Guid id, Guid enterpriseId, ClaimsPrincipal userNowFromJwt);

    public Task DeleteCarrierEnterpriseOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt);

    public Task AddProducerEnterpriseOfProductAsync(Guid id, Guid enterpriseId, ClaimsPrincipal userNowFromJwt);

    public Task DeleteProducerEnterpriseOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt);

    public Task AddResponsibleUserOfProductAsync(Guid id, string userId, ClaimsPrincipal userNowFromJwt);

    public Task DeleteResponsibleUserOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt);

    public Task AddFactoryOfProductAsync(Guid id, Guid factoryId, ClaimsPrincipal userNowFromJwt);

    public Task DeleteFactoryOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt);

    public Task UploadImagesAsync(Guid id, List<IFormFile> listFiles, ClaimsPrincipal userNowFromJwt);

    public Task DeleteImageAsync(Guid id, Guid fileId, ClaimsPrincipal userNowFromJwt);
}