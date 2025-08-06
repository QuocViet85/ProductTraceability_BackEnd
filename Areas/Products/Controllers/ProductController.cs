using App.Areas.Auth.AuthorizationType;
using App.Areas.Products.DTO;
using App.Areas.Products.Services;
using App.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Products.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = $"{Roles.ADMIN}, {Roles.ENTERPRISE}")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet("get-many")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMany(int pageNumber, int limit, string? search)
    {
        try
        {
            var result = await _productService.GetManyAsync(pageNumber, limit, search);

            return Ok(new
            {
                totalProducts = result.totalItems,
                products = result.listDTOs
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("get-one-by-id/{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOneById(Guid id)
    {
        try
        {
            var productDTO = await _productService.GetOneByIdAsync(id);

            return Ok(productDTO);
        }
        catch
        {
            throw;
        }
    }

    [HttpGet("get-one-by-code/{traceCode}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetOneByTraceCode(string traceCode)
    {
        try
        {
            var productDTO = await _productService.GetOneByTraceCodeAsync(traceCode);

            return Ok(productDTO);
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
            var result = await _productService.GetMyManyAsync(User, pageNumber, limit, search);

            return Ok(new
            {
                totalEnterprises = result.totalItems,
                factories = result.listDTOs
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] ProductDTO factoryDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _productService.CreateAsync(factoryDTO, User);

                return Ok("Tạo sản phẩm thành công");
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
    public async Task<IActionResult> Update(Guid id, [FromBody] ProductDTO factoryDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _productService.UpdateAsync(id, factoryDTO, User);

                return Ok("Cập nhật sản phẩm thành công");
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
            await _productService.DeleteAsync(id, User);

            return Ok("Xóa sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("add-ownership/{id}")]
    public async Task<IActionResult> AddOwnerIndividualEnterpriseOfProduct(Guid id, [FromBody] string userId)
    {
        try
        {
            await _productService.AddOwnerIndividualEnterpriseOfProductAsync(id, userId, User);

            return Ok("Thêm hộ kinh doanh cá nhân sở hữu sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("delete-ownership/{id}")]
    public async Task<IActionResult> DeleteOwnerIndividualEnterpriseOfProduct(Guid id)
    {
        try
        {
            await _productService.DeleteOwnerIndividualEnterpriseOfProductAsync(id, User);

            return Ok("Xóa hộ kinh doanh cá nhân sở hữu sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("add-owner-enterprise/{id}")]
    public async Task<IActionResult> AddOwnerEnterpriseOfProduct(Guid id, [FromBody] Guid enterpriseId)
    {
        try
        {
            await _productService.AddOwnerEnterpriseOfProductAsync(id, enterpriseId, User);

            return Ok("Thêm doanh nghiệp sở hữu sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("delete-owner-enterprise/{id}")]
    public async Task<IActionResult> DeleteOwnerEnterpriseOfProduct(Guid id)
    {
        try
        {
            await _productService.DeleteOwnerEnterpriseOfProductAsync(id, User);

            return Ok("Xóa doanh nghiệp sở hữu sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("add-producer-enterprise/{id}")]
    public async Task<IActionResult> AddProducerEnterpriseOfProduct(Guid id, [FromBody] Guid enterpriseId)
    {
        try
        {
            await _productService.AddProducerEnterpriseOfProductAsync(id, enterpriseId, User);

            return Ok("Thêm doanh nghiệp sản xuất sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("delete-producer-enterprise/{id}")]
    public async Task<IActionResult> DeleteProducerEnterpriseOfProduct(Guid id)
    {
        try
        {
            await _productService.DeleteProducerEnterpriseOfProductAsync(id, User);

            return Ok("Xóa doanh nghiệp sản xuất sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("add-carrier-enterprise/{id}")]
    public async Task<IActionResult> AddCarrierEnterpriseOfProduct(Guid id, [FromBody] Guid enterpriseId)
    {
        try
        {
            await _productService.AddCarrierEnterpriseOfProductAsync(id, enterpriseId, User);

            return Ok("Thêm doanh nghiệp vận tải sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("delete-carrier-enterprise/{id}")]
    public async Task<IActionResult> DeleteEnterpriseOfProduct(Guid id)
    {
        try
        {
            await _productService.DeleteCarrierEnterpriseOfProductAsync(id, User);

            return Ok("Xóa doanh nghiệp vận tải sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("add-responsible-user/{id}")]
    public async Task<IActionResult> AddResponsibleUserOfProduct(Guid id, [FromBody] string userId)
    {
        try
        {
            await _productService.AddResponsibleUserOfProductAsync(id, userId, User);

            return Ok("Thêm người phụ trách sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("delete-responsible-user/{id}")]
    public async Task<IActionResult> DeleteResponsibleUserOfProduct(Guid id)
    {
        try
        {
            await _productService.DeleteResponsibleUserOfProductAsync(id, User);

            return Ok("Xóa người phụ trách sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("add-factory/{id}")]
    public async Task<IActionResult> AddFactoryOfProduct(Guid id, [FromBody] Guid enterpriseId)
    {
        try
        {
            await _productService.AddFactoryOfProductAsync(id, enterpriseId, User);

            return Ok("Thêm nhà máy của sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPut("delete-factory/{id}")]
    public async Task<IActionResult> DeleteFactoryOfProduct(Guid id)
    {
        try
        {
            await _productService.DeleteFactoryOfProductAsync(id, User);

            return Ok("Xóa nhà máy của sản phẩm thành công");
        }
        catch
        {
            throw;
        }
    }

    [HttpPost("upload-photos/{id}")]
    public async Task<IActionResult> UploadPhotosOfProduct(Guid id, List<IFormFile> listFiles)
    {
        try
        {
            await _productService.UploadPhotosOfProductAsync(id, listFiles, User);

            return Ok("Upload ảnh thành công");
        }
        catch
        {
            throw;
        }
    }
    
    [HttpPut("delete-photos/{id}")]
    public async Task<IActionResult> DeletePhotoOfProduct(Guid id, Guid fileId)
    {
        try
        {
            await _productService.DeletePhotoOfProductAsync(id, fileId, User);

            return Ok("Xóa ảnh thành công");
        }
        catch
        {
            throw;
        }
    }
}