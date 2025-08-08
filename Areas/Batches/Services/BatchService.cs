using System.Security.Claims;
using App.Areas.Auth.Mapper;
using App.Areas.Batches.DTO;
using App.Areas.Batches.Mapper;
using App.Areas.Batches.Models;
using App.Areas.Batches.Repositories;
using App.Areas.Factories.Mapper;
using App.Areas.Factories.Repositories;
using App.Areas.Products.Authorization;
using App.Areas.Products.Mapper;
using App.Areas.Products.Repositories;
using App.Helper;
using Microsoft.AspNetCore.Authorization;

namespace App.Areas.Batches.Services;

public class BatchService : IBatchService
{
    private readonly IBatchRepository _batchRepo;
    private readonly IProductRepository _productRepo;
    private readonly IFactoryRepository _factoryRepo;
    private readonly IAuthorizationService _authorizationService;

    public BatchService(IBatchRepository batchRepo, IProductRepository productRepo, IFactoryRepository factoryRepo, IAuthorizationService authorizationService)
    {
        _batchRepo = batchRepo;
        _productRepo = productRepo;
        _factoryRepo = factoryRepo;
        _authorizationService = authorizationService;
    }

    public async Task<(int totalItems, List<BatchDTO> listDTOs)> GetManyByProductAsync(Guid productId, int pageNumber, int limit, string search, bool descending)
    {
        int totalBatches = await _batchRepo.GetTotalByProductAsync(productId);
        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<BatchModel> listBatches = await _batchRepo.GetManyByProductAsync(productId, pageNumber, limit, search, descending);
        List<BatchDTO> listBatchDTOs = new List<BatchDTO>();
        foreach (var batch in listBatches)
        {
            var batchDTO = BatchMapper.ModelToDto(batch);
            AddRelationToDTO(batchDTO, batch);
            listBatchDTOs.Add(batchDTO);
        }

        return (totalBatches, listBatchDTOs);
    }

    public async Task<BatchDTO> GetOneByIdAsync(Guid id)
    {
        var batch = await _batchRepo.GetOneByIdAsync(id);

        if (batch == null)
        {
            throw new Exception("Không tìm thấy lô hàng");
        }

        var batchDTO = BatchMapper.ModelToDto(batch);
        AddRelationToDTO(batchDTO, batch);
        return batchDTO;
    }

    public async Task<BatchDTO> GetOneByBatchCodeAsync(string batchCode)
    {
        var batch = await _batchRepo.GetOneByBatchCodeAsync(batchCode);

        if (batch == null)
        {
            throw new Exception("Không tìm thấy lô hàng");
        }

        var batchDTO = BatchMapper.ModelToDto(batch);
        AddRelationToDTO(batchDTO, batch);
        return batchDTO;
    }

    public async Task CreateAsync(BatchDTO batchDTO, ClaimsPrincipal userNowFromJwt)
    {
        if (batchDTO.ProductId == null)
        {
            throw new Exception("Phải nhập sản phẩm cho lô hàng");
        }
        Guid productId = (Guid)batchDTO.ProductId;
        var product = await _productRepo.GetOneByIdAsync(productId);

        if (product == null)
        {
            throw new Exception("Không tồn tại sản phẩm nền không thể thêm lô hàng cho sản phẩm");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, product, new CanUpdateProductRequirement(product.TraceCode)); //Bản chất của lô hàng vẫn là sửa sản phẩm nên dùng luôn Auth của sửa sản phẩm

        if (checkAuth.Succeeded)
        {
            var batch = BatchMapper.DtoToModel(batchDTO);
            batch.ProductId = productId;

            string batchCode = "";
            if (batchDTO.BatchCode != null)
            {
                bool existBatchCode = await _batchRepo.CheckExistByBatchCodeAsync(batchDTO.BatchCode);

                if (existBatchCode)
                {
                    throw new Exception("Mã lô sản phẩm đã tồn tại. Vui lòng chọn mã khác hoặc để hệ thống tự sinh mã");
                }

                batchCode = batchDTO.BatchCode;
            }
            else
            {
                batchCode = CreateCode.GenerateCodeFromTicks();
            }
            batch.BatchCode = batchCode;

            if (batchDTO.FactoryId != null)
            {
                bool existFactory = await _factoryRepo.CheckExistByIdAsync((Guid)batchDTO.FactoryId);
                if (existFactory)
                {
                    batch.FactoryId = batchDTO.FactoryId;
                }
            }

            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            batch.CreatedUserId = userIdNow;
            batch.CreatedAt = DateTime.Now;

            var result = await _batchRepo.CreateAsync(batch);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Tạo lô hàng thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền tạo lô hàng với sản phẩm này");
        }
        
    }

    public async Task UpdateAsync(Guid id, BatchDTO batchDTO, ClaimsPrincipal userNowFromJwt)
    {
        var batch = await _batchRepo.GetOneByIdAsync(id);

        if (batch == null)
        {
            throw new Exception("Không tồn tại lô hàng");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, batch.Product, new CanUpdateProductRequirement(batch.Product.TraceCode)); //Bản chất của lô hàng vẫn là sửa sản phẩm nên dùng luôn Auth của sửa sản phẩm

        if (checkAuth.Succeeded)
        {
            batch = BatchMapper.DtoToModel(batchDTO, batch);

            string batchCode = batch.BatchCode;
            if (batchDTO.BatchCode != null)
            {
                bool existBatchCode = await _batchRepo.CheckExistExceptThisByBatchCodeAsync(id, batch.BatchCode);

                if (existBatchCode)
                {
                    throw new Exception("Mã lô sản phẩm đã tồn tại. Vui lòng chọn mã khác hoặc để hệ thống tự sinh mã");
                }

                batchCode = batchDTO.BatchCode;
            }
            batch.BatchCode = batchCode;

            if (batchDTO.FactoryId != null)
            {
                bool existFactory = await _factoryRepo.CheckExistByIdAsync((Guid)batchDTO.FactoryId);
                if (existFactory)
                {
                    batch.FactoryId = batchDTO.FactoryId;
                }
            }
            else
            {
                batch.FactoryId = batchDTO.FactoryId;
            }

            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            batch.UpdatedUserId = userIdNow;
            batch.UpdatedAt = DateTime.Now;

            var result = await _batchRepo.UpdateAsync(batch);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Tạo lô hàng thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền sửa lô hàng của sản phẩm này");
        }
    }

    public async Task DeleteAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var batch = await _batchRepo.GetOneByIdAsync(id);

        if (batch == null)
        {
            throw new Exception("Không tồn tại lô hàng");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, batch.Product, new CanUpdateProductRequirement(batch.Product.TraceCode)); //Bản chất của lô hàng vẫn là sửa sản phẩm nên dùng luôn Auth của sửa sản phẩm

        if (checkAuth.Succeeded)
        {
            var result = await _batchRepo.DeleteAsync(batch);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa lô hàng thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa lô hàng của sản phẩm này");
        }
    }

    private void AddRelationToDTO(BatchDTO batchDTO, BatchModel batch)
    {
        if (batch.Product != null)
        {
            batchDTO.Product = ProductMapper.ModelToDto(batch.Product);
        }

        if (batch.Factory != null)
        {
            batchDTO.Factory = FactoryMapper.ModelToDto(batch.Factory);
        }

        if (batch.CreatedUser != null)
        {
            batchDTO.CreatedUser = UserMapper.ModelToDto(batch.CreatedUser);
        }

        if (batch.UpdatedUser != null)
        {
            batchDTO.UpdatedUser = UserMapper.ModelToDto(batch.UpdatedUser);
        }
    }

    //Not Implement
    public Task<(int totalItems, List<BatchDTO> listDTOs)> GetManyAsync(int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }

    public Task<(int totalItems, List<BatchDTO> listDTOs)> GetMyManyAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }
}