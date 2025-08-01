using System.Security.Claims;
using App.Areas.Auth.Mapper;
using App.Areas.Categories.Mapper;
using App.Areas.Enterprises.Mapper;
using App.Areas.Factories.Mapper;
using App.Areas.IndividualEnterprises.Mapper;
using App.Areas.Products.DTO;
using App.Areas.Products.Mapper;
using App.Areas.Products.Models;
using App.Areas.Products.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace App.Areas.Products.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepo;
    private readonly IAuthorizationService _authorizationService;

    public ProductService(IProductRepository productRepo, IAuthorizationService authorizationService)
    {
        _productRepo = productRepo;
        _authorizationService = authorizationService;
    }

    public Task<(int totalItems, List<ProductDTO> listDTOs)> GetManyAsync(int pageNumber, int limit, string search)
    {
        throw new NotImplementedException();
    }

    public async Task<(int totalItems, List<ProductDTO> listDTOs)> GetMyManyAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search)
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

    public Task<ProductDTO> GetOneByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<(int totalProducts, List<ProductDTO> productDTOs)> GetOneByTraceCodeAsync(string traceCode)
    {
        throw new NotImplementedException();
    }

    public Task<List<(int totalProducts, List<ProductDTO> productDTOs)>> GetManyByCarrierEnterpriseAsync(Guid enterpriseId, int pageNumber, int limit, string search)
    {
        throw new NotImplementedException();
    }

    public Task<List<(int totalProducts, List<ProductDTO> productDTOs)>> GetManyByCategoryAsync(Guid categoryId, int pageNumber, int limit, string search)
    {
        throw new NotImplementedException();
    }

    public Task<List<(int totalProducts, List<ProductDTO> productDTOs)>> GetManyByOwnerEnterpriseAsync(Guid enterpriseId, int pageNumber, int limit, string search)
    {
        throw new NotImplementedException();
    }

    public Task<List<(int totalProducts, List<ProductDTO> productDTOs)>> GetManyByOwnerIndividualEnterpriseAsync(string individualEnterpriseId, int pageNumber, int limit, string search)
    {
        throw new NotImplementedException();
    }

    public Task<List<(int totalProducts, List<ProductDTO> productDTOs)>> GetManyByProducerEnterpriseAsync(Guid enterpriseId, int pageNumber, int limit, string search)
    {
        throw new NotImplementedException();
    }

    public Task<List<(int totalProducts, List<ProductDTO> productDTOs)>> GetManyByProductAsync(Guid factoryId, int pageNumber, int limit, string search)
    {
        throw new NotImplementedException();
    }

    public Task<List<(int totalProducts, List<ProductDTO> productDTOs)>> GetManyByResponsibleUserAsync(string userId, int pageNumber, int limit, string search)
    {
        throw new NotImplementedException();
    }

    public Task CreateAsync(ProductDTO TDto, ClaimsPrincipal userNowFromJwt)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Guid id, ProductDTO TDto, ClaimsPrincipal userNowFromJwt)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        throw new NotImplementedException();
    }

    public Task AddOwnerIndividualEnterpriseOfProductAsync(Guid id, string individualEnterpriseId, ClaimsPrincipal userNowFromJwt)
    {
        throw new NotImplementedException();
    }

    public Task DeleteOwnerIndividualEnterpriseOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        throw new NotImplementedException();
    }

    public Task AddOwnerEnterpriseOfProductAsync(Guid id, Guid enterpriseId, ClaimsPrincipal userNowFromJwt)
    {
        throw new NotImplementedException();
    }

    public Task DeleteOwnerEnterpriseOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        throw new NotImplementedException();
    }

    public Task AddCarrierEnterpriseOfProductAsync(Guid id, Guid enterpriseId, ClaimsPrincipal userNowFromJwt)
    {
        throw new NotImplementedException();
    }

    public Task DeleteCarrierEnterpriseOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        throw new NotImplementedException();
    }

    public Task AddProducerEnterpriseOfProductAsync(Guid id, Guid enterpriseId, ClaimsPrincipal userNowFromJwt)
    {
        throw new NotImplementedException();
    }

    public Task DeleteProducerEnterpriseOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        throw new NotImplementedException();
    }

    public Task AddFactoryOfProductAsync(Guid id, string factoryId, ClaimsPrincipal userNowFromJwt)
    {
        throw new NotImplementedException();
    }

    public Task DeleteFactoryOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        throw new NotImplementedException();
    }

    public Task AddResponsibleUserOfProductAsync(Guid id, string userId, ClaimsPrincipal userNowFromJwt)
    {
        throw new NotImplementedException();
    }

    public Task DeleteResponsibleUserEnterpriseOfProductAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        throw new NotImplementedException();
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