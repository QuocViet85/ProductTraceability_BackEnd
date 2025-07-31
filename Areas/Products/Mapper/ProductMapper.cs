using App.Areas.Products.DTO;
using App.Areas.Products.Models;

namespace App.Areas.Products.Mapper;

public static class ProductMapper
{
    public static ProductDTO ModelToDto(ProductModel product)
    {
        return new ProductDTO()
        {
            Id = product.Id,
            TraceCode = product.TraceCode,
            Description = product.Description,
            Website = product.Website,
            Price = product.Price,
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    } 

    public static ProductModel DtoToModel(ProductDTO productDTO, ProductModel productUpdate = null)
    {
        ProductModel product;

        if (productUpdate == null)
        {
            product = new ProductModel();
        }
        else
        {
            product = productUpdate;
        }

        product.Description = productDTO.Description;
        product.Website = productDTO.Website;
        product.Price = productDTO.Price;

        return product;
    }
}