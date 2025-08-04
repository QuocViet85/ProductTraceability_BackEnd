using App.Areas.Batches.DTO;
using App.Areas.Batches.Models;

namespace App.Areas.Batches.Mapper;

public static class BatchMapper
{
    public static BatchDTO ModelToDto(BatchModel batch)
    {
        return new BatchDTO()
        {
            Id = batch.Id,
            BatchCode = batch.BatchCode,
            Name = batch.Name,
            ManufactureDate = batch.ManufactureDate,
            ExpireDate = batch.ExpireDate,
            Quantity = batch.Quantity,
            Status = batch.Status,
        };
    }

    public static BatchModel DtoToModel(BatchDTO batchDTO, BatchModel batchUpdate = null)
    {
        BatchModel batch;
        if (batchUpdate == null)
        {
            batch = new BatchModel();
        }
        else
        {
            batch = batchUpdate;
        }

        batch.Name = batchDTO.Name;
        batch.ManufactureDate = batchDTO.ManufactureDate;
        batch.ExpireDate = batchDTO.ExpireDate;
        batch.Quantity = batchDTO.Quantity;
        batch.Status = batchDTO.Status;

        return batch;
    }
}