using App.Areas.Comments.Models;
using App.Areas.Comments.Services;
using App.Areas.Products.DTO;
using App.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Areas.Comments.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommentController : ControllerBase
{
    private readonly ICommentService _commentService;

    public CommentController(ICommentService commentService)
    {
        _commentService = commentService;
    }

    [HttpGet("get-many/{productId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMany(Guid productId, int pageNumber, int limit)
    {
        try
        {
            var result = await _commentService.GetManyByProductAsync(productId, pageNumber, limit);

            return Ok(new
            {
                totalComments = result.totalItems,
                comments = result.listDTOs
            });
        }
        catch
        {
            throw;
        }
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CommentDTO factoryDTO)
    {
        try
        {
            if (ModelState.IsValid)
            {
                await _commentService.CreateAsync(factoryDTO, User);

                return Ok("Tạo bình luận thành công");
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
            await _commentService.DeleteAsync(id, User);

            return Ok("Xóa bình luận thành công");
        }
        catch
        {
            throw;
        }
    }

}