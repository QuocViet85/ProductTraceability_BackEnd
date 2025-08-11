using System.Security.Claims;
using App.Areas.Auth.Mapper;
using App.Areas.Categories.DTO;
using App.Areas.Categories.Mapper;
using App.Areas.Categories.Models;
using App.Areas.Categories.Repositories;

namespace App.Areas.Categories.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categoryRepo;

    public CategoryService(ICategoryRepository categoryRepo)
    {
        _categoryRepo = categoryRepo;
    }

    public async Task<(int totalItems, List<CategoryDTO> listDTOs)> GetManyAsync(int pageNumber, int limit, string search, bool descending)
    {
        int totalCategories = await _categoryRepo.GetTotalAsync();

        List<CategoryModel> listCategories = await _categoryRepo.GetManyAsync(pageNumber, limit, search, descending);

        List<CategoryDTO> listCategoryDTOs = new List<CategoryDTO>();

        foreach (var category in listCategories)
        {
            var categoryDTO = CategoryMapper.ModelToDto(category);
            AddRelationToDTO(categoryDTO, category);
            listCategoryDTOs.Add(categoryDTO);
        }

        return (totalCategories, listCategoryDTOs);
    }

    public async Task<CategoryDTO> GetOneByIdAsync(Guid id)
    {
        var category = await _categoryRepo.GetOneByIdAsync(id);

        if (category == null)
        {
            throw new Exception("Không tìm thấy danh mục sản phẩm");
        }

        var categoryDTO = CategoryMapper.ModelToDto(category);
        AddRelationToDTO(categoryDTO, category);

        return categoryDTO;
    }
    public async Task CreateAsync(CategoryDTO categoryDTO, ClaimsPrincipal userNowFromJwt)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (await _categoryRepo.CheckExistByNameAsync(categoryDTO.Name))
        {
            throw new Exception("Không thể tạo danh mục sản phẩm vì đã có danh mục sản phẩm cùng tên");
        }

        var category = CategoryMapper.DtoToModel(categoryDTO);

        if (categoryDTO.IsParent)
        {
            category.IsParent = true;
            category.ParentCategoryId = null;
        }
        else if (categoryDTO.ParentCategoryId != null)
        {
            if (!await _categoryRepo.CheckExistByIdAsync((Guid)categoryDTO.ParentCategoryId))
            {
                throw new Exception("Danh mục cha lựa chọn không tồn tại nên không thể tạo danh mục");
            }
            category.IsParent = false;
            category.ParentCategoryId = categoryDTO.ParentCategoryId;
        }
        else
        {
            throw new Exception("Chưa chọn là danh mục cha hoặc có danh mục cha nên không thể tạo danh mục");
        }

        category.CreatedUserId = Guid.Parse(userIdNow);
        category.CreatedAt = DateTime.Now;

        int result = await _categoryRepo.CreateAsync(category);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Tạo danh mục sản phẩm thất bại");
        }
    }

    public async Task DeleteAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var category = await _categoryRepo.GetOneByIdAsync(id);

        if (category == null)
        {
            throw new Exception("Danh mục sản phẩm không tồn tại");
        }

        int result = await _categoryRepo.DeleteAsync(category);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Xóa danh mục sản phẩm thất bại");
        }
    }

    public async Task UpdateAsync(Guid id, CategoryDTO categoryDTO, ClaimsPrincipal userNowFromJwt)
    {
        var category = await _categoryRepo.GetOneByIdAsync(id);

        if (category == null)
        {
            throw new Exception("Danh mục sản phẩm không tồn tại");
        }

        if (await _categoryRepo.CheckExistExceptThisByNameAsync(id, categoryDTO.Name))
        {
            throw new Exception("Không cập nhật danh mục sản phẩm vì đã có danh mục sản phẩm khác cùng tên");
        }

        category = CategoryMapper.DtoToModel(categoryDTO, category);
        category.UpdatedUserId = Guid.Parse(userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        category.UpdatedAt = DateTime.Now;

        int result = await _categoryRepo.UpdateAsync(category);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật danh mục sản phẩm thất bại");
        }
    }

    public async Task<CategoryDTO> GetOneByNameAsync(string name)
    {
        var category = await _categoryRepo.GetOneByNameAsync(name);

        if (category == null)
        {
            throw new Exception("Không tìm thấy danh mục sản phẩm");
        }

        var categoryDTO = CategoryMapper.ModelToDto(category);
        AddRelationToDTO(categoryDTO, category);

        return categoryDTO;
    }

    private void AddRelationToDTO(CategoryDTO categoryDTO, CategoryModel category)
    {
        if (category.CreatedUser != null)
        {
            categoryDTO.CreatedUser = UserMapper.ModelToDto(category.CreatedUser);
        }

        if (category.UpdatedUser != null)
        {
            categoryDTO.UpdatedUser = UserMapper.ModelToDto(category.UpdatedUser);
        }

        if (category.ParentCategory != null)
        {
            categoryDTO.ParentCategory = CategoryMapper.ModelToDto(category.ParentCategory);
        }

        if (category.ChildCategories != null)
        {
            categoryDTO.ChildCategories = new List<CategoryDTO>();
            foreach (var childCategory in category.ChildCategories)
            {
                categoryDTO.ChildCategories.Add(CategoryMapper.ModelToDto(childCategory));
            }
        }
    }

    public async Task<(int totalItems, List<CategoryDTO> listDTOs)> GetMyManyAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }
}