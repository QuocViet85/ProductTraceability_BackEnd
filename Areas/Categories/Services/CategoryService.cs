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

    public async Task<(int totalItems, List<CategoryDTO> listDTOs)> GetManyAsync(int pageNumber, int limit, string search)
    {
        int totalCategories = await _categoryRepo.GetTotalAsync();

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<CategoryModel> listCategories = await _categoryRepo.GetManyAsync(pageNumber, limit, search);

        List<CategoryDTO> listCategoryDTOs = new List<CategoryDTO>();

        foreach (var category in listCategories)
        {
            var categoryDTO = CategoryMapper.ModelToDto(category);
            AddRelationToDTO(categoryDTO, category);
            listCategoryDTOs.Add(categoryDTO);
        }

        return (totalCategories, listCategoryDTOs);
    }

    public async Task<(int totalItems, List<CategoryDTO> listDTOs)> GetMyManyAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search)
    {
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        int totalMyCategories = await _categoryRepo.GetMyTotalAsync(userIdNow);

        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<CategoryModel> listCategories = await _categoryRepo.GetMyManyAsync(userIdNow, pageNumber, limit, search);

        List<CategoryDTO> listCategoryDTOs = new List<CategoryDTO>();

        foreach (var category in listCategories)
        {
            var categoryDTO = CategoryMapper.ModelToDto(category);
            AddRelationToDTO(categoryDTO, category);
            listCategoryDTOs.Add(categoryDTO);
        }

        return (totalMyCategories, listCategoryDTOs);
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

        if (await _categoryRepo.CheckExistAsync(categoryDTO.Name))
        {
            throw new Exception("Không thể tạo danh mục sản phẩm vì đã có danh mục sản phẩm cùng tên");
        }

        var category = CategoryMapper.DtoToModel(categoryDTO);
        category.CreatedUserId = userIdNow;
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

        category = CategoryMapper.DtoToModel(categoryDTO, category);

        int result = await _categoryRepo.UpdateAsync(category);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Cập nhật danh mục sản phẩm thất bại");
        }
    }

    private void AddRelationToDTO(CategoryDTO categoryDTO, CategoryModel category)
    {
        if (category.CreatedUser != null)
        {
            categoryDTO.CreatedUser = UserMapper.ModelToDto(category.CreatedUser);
        }
    }
}