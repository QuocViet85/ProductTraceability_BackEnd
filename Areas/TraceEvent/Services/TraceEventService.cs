using System.Security.Claims;
using App.Areas.Auth.Mapper;
using App.Areas.Batches.Mapper;
using App.Areas.Batches.Repositories;
using App.Areas.Products.Authorization;
using App.Areas.TraceEvents.DTO;
using App.Areas.TraceEvents.Mapper;
using App.Areas.TraceEvents.Models;
using App.Areas.TraceEvents.Repositories;
using App.Helper;
using Microsoft.AspNetCore.Authorization;

namespace App.Areas.TraceEvents.Services;

public class TraceEventService : ITraceEventService
{
    private readonly ITraceEventRepository _traceEventRepo;

    private readonly IBatchRepository _batchRepo;

    private readonly IAuthorizationService _authorizationService;

    public TraceEventService(ITraceEventRepository traceEventRepo, IBatchRepository batchRepo, IAuthorizationService authorizationService)
    {
        _traceEventRepo = traceEventRepo;
        _batchRepo = batchRepo;
        _authorizationService = authorizationService;
    }

    public async Task<(int totalItems, List<TraceEventDTO> listDTOs)> GetManyByBatchAsync(Guid batchId, int pageNumber, int limit, string search, bool descending)
    {
        int totalTraceEvents = await _traceEventRepo.GetTotalByBatchAsync(batchId);
        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<TraceEventModel> listTraceEvents = await _traceEventRepo.GetManyByBatchAsync(batchId, pageNumber, limit, search, descending);
        List<TraceEventDTO> listTraceEventDTOs = new List<TraceEventDTO>();
        foreach (var traceEvent in listTraceEvents)
        {
            var traceEventDTO = TraceEventMapper.ModelToDto(traceEvent);
            AddRelationToDTO(traceEventDTO, traceEvent);
            listTraceEventDTOs.Add(traceEventDTO);
        }

        return (totalTraceEvents, listTraceEventDTOs);
    }

    public async Task<TraceEventDTO> GetOneByIdAsync(Guid id)
    {
        var traceEvent = await _traceEventRepo.GetOneByIdAsync(id);

        if (traceEvent == null)
        {
            throw new Exception("Không tìm thấy sự kiện truy xuất");
        }

        var traceEventDTO = TraceEventMapper.ModelToDto(traceEvent);
        AddRelationToDTO(traceEventDTO, traceEvent);

        return traceEventDTO;
    }

    public async Task<TraceEventDTO> GetOneByTraceEventCodeAsync(string traceEventCode)
    {
        var traceEvent = await _traceEventRepo.GetOneByTraceEventCodeAsync(traceEventCode);

        if (traceEvent == null)
        {
            throw new Exception("Không tìm thấy sự kiện truy xuất");
        }

        var traceEventDTO = TraceEventMapper.ModelToDto(traceEvent);
        AddRelationToDTO(traceEventDTO, traceEvent);

        return traceEventDTO;
    }

    public async Task CreateAsync(TraceEventDTO traceEventDTO, ClaimsPrincipal userNowFromJwt)
    {
        if (traceEventDTO.BatchId == null)
        {
            throw new Exception("Phải nhập lô hàng cho sự kiện truy xuất");
        }

        Guid batchId = (Guid)traceEventDTO.BatchId;
        var batch = await _batchRepo.GetOneByIdAsync(batchId);

        if (batch == null)
        {
            throw new Exception("Không tồn tại lô hàng nền không thể thêm sự kiện cho lô hàng");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, batch.Product, new CanUpdateProductRequirement(batch.Product.TraceCode)); //Bản chất của sự kiện truy xuất vẫn là sửa sản phẩm nên dùng luôn Auth của sửa sản phẩm

        if (checkAuth.Succeeded)
        {
            var traceEvent = TraceEventMapper.DtoToModel(traceEventDTO);
            traceEvent.BatchId = batchId;

            string traceEventCode = "";
            if (traceEventDTO.TraceEventCode != null)
            {
                bool existTraceEventCode = await _traceEventRepo.CheckExistByTraceEventCodeAsync(traceEventDTO.TraceEventCode);

                if (existTraceEventCode)
                {
                    throw new Exception("Mã sự kiện truy xuất đã tồn tại. Vui lòng chọn mã khác hoặc để hệ thống tự sinh mã");
                }

                traceEventCode = traceEventDTO.TraceEventCode;
            }
            else
            {
                traceEventCode = CreateCode.GenerateCodeFromTicks();
            }
            traceEvent.TraceEventCode = traceEventCode;

            if (traceEventDTO.TimeStamp == null)
            {
                traceEvent.TimeStamp = DateTime.Now;
            }

            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            traceEvent.CreatedUserId = userIdNow;
            traceEvent.CreatedAt = DateTime.Now;

            var result = await _traceEventRepo.CreateAsync(traceEvent);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Tạo sự kiện truy xuất thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền tạo sự kiên truy xuất với lô hàng này");
        }
    }

    public async Task UpdateAsync(Guid id, TraceEventDTO traceEventDTO, ClaimsPrincipal userNowFromJwt)
    {
        var traceEvent = await _traceEventRepo.GetOneByIdAsync(id);

        if (traceEvent == null)
        {
            throw new Exception("Không tồn tại sự kiện truy xuất");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, traceEvent.Batch.Product, new CanUpdateProductRequirement(traceEvent.Batch.Product.TraceCode)); //Bản chất của sự kiện truy xuất vẫn là sửa sản phẩm nên dùng luôn Auth của sửa sản phẩm

        if (checkAuth.Succeeded)
        {
            traceEvent = TraceEventMapper.DtoToModel(traceEventDTO, traceEvent);

            string traceEventCode = traceEvent.TraceEventCode;
            if (traceEventDTO.TraceEventCode != null)
            {
                bool existTraceEventCode = await _traceEventRepo.CheckExistExceptThisByTraceEventCodeAsync(id, traceEventDTO.TraceEventCode);

                if (existTraceEventCode)
                {
                    throw new Exception("Mã sự kiện truy xuất đã tồn tại. Vui lòng chọn mã khác hoặc để hệ thống tự sinh mã");
                }

                traceEventCode = traceEventDTO.TraceEventCode;
            }
            traceEvent.TraceEventCode = traceEventCode;

            var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            traceEvent.UpdatedUserId = userIdNow;
            traceEvent.UpdatedAt = DateTime.Now;

            var result = await _traceEventRepo.UpdateAsync(traceEvent);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Tạo sự kiện truy xuất thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền sửa sự kiện truy xuất của lô hàng này");
        }
    }

    public async Task DeleteAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var traceEvent = await _traceEventRepo.GetOneByIdAsync(id);

        if (traceEvent == null)
        {
            throw new Exception("Không tồn tại lô hàng");
        }

        var checkAuth = await _authorizationService.AuthorizeAsync(userNowFromJwt, traceEvent.Batch.Product, new CanUpdateProductRequirement(traceEvent.Batch.Product.TraceCode)); //Bản chất của lô hàng vẫn là sửa sản phẩm nên dùng luôn Auth của sửa sản phẩm

        if (checkAuth.Succeeded)
        {
            var result = await _traceEventRepo.DeleteAsync(traceEvent);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Xóa sự kiện truy xuất thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa sự kiện truy xuất này");
        }
    }

    private void AddRelationToDTO(TraceEventDTO traceEventDTO, TraceEventModel traceEvent)
    {
        if (traceEvent.Batch != null)
        {
            traceEventDTO.Batch = BatchMapper.ModelToDto(traceEvent.Batch);
        }

        if (traceEvent.CreatedUser != null)
        {
            traceEventDTO.CreatedUser = UserMapper.ModelToDto(traceEvent.CreatedUser);
        }

        if (traceEvent.UpdatedUser != null)
        {
            traceEventDTO.UpdatedUser = UserMapper.ModelToDto(traceEvent.UpdatedUser);
        }
    }

    //Not Implement

    public Task<(int totalItems, List<TraceEventDTO> listDTOs)> GetManyAsync(int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }

    public Task<(int totalItems, List<TraceEventDTO> listDTOs)> GetMyManyAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }
}