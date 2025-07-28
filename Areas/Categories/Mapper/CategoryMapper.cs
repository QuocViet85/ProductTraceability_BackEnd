using App.Areas.Categories.DTO;
using App.Areas.Categories.Models;

namespace App.Areas.Categories.Mapper;

public static class CategoryMapper
{
    public static CategoryDTO ModelToDto(CategoryModel category)
    {
        return new CategoryDTO()
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            IsDefault = category.IsDefault,
            CreatedAt = category.CreatedAt
        };
    }

    public static CategoryModel DtoToModel(CategoryDTO categoryDTO, CategoryModel categoryUpdate = null)
    {
        CategoryModel category;

        if (categoryUpdate == null)
        {
            category = new CategoryModel();
        }
        else
        {
            category = categoryUpdate;
        }

        category.Name = categoryDTO.Name;
        category.Description = categoryDTO.Description;

        if (categoryDTO.IsDefault != null)
        {
            category.IsDefault = (bool)categoryDTO.IsDefault;
        }

        return category;
    }
}