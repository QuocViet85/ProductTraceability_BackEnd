using App.Areas.Auth.AuthorizationType;
using App.Areas.Categories.DTO;
using App.Areas.Categories.Services;
using App.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Categories.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{Roles.ADMIN}")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet("get-many")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMany(int pageNumber, int limit, string? search)
    {
        try
        {
            var result = await _categoryService.GetManyAsync(pageNumber, limit, search);

            return Ok(new
            {
                totalCategories = result.totalItems,
                categories = result.listDTOs
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("get-one/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOne(Guid id)
    {
        try
        {
            var categoryDTO = await _categoryService.GetOneByIdAsync(id);

            return Ok(categoryDTO);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("get-my-many")]
    public async Task<IActionResult> GetMyMany(int pageNumber, int limit, string? search)
    {
        try
        {
            var result = await _categoryService.GetMyManyAsync(User, pageNumber, limit, search);

            return Ok(new
            {
                totalEnterprises = result.totalItems,
                categories = result.listDTOs
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CategoryDTO categoryDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _categoryService.CreateAsync(categoryDTO, User);

                return Ok("Tạo danh mục sản phẩm thành công");
            }
            else
            {
                return BadRequest(ErrorMessage.DTO(ModelState));
            }
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("update/{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CategoryDTO categoryDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _categoryService.UpdateAsync(id, categoryDTO, User);

                return Ok("Cập nhật danh mục sản phẩm thành công");
            }
            else
            {
                return BadRequest(ErrorMessage.DTO(ModelState));
            }
        }
        catch
        {
            throw;
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await _categoryService.DeleteAsync(id, User);

            return Ok("Xóa danh mục sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }
}