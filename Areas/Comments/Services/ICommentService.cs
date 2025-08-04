using System.Security.Claims;
using App.Areas.Comments.Models;
using App.Services;

namespace App.Areas.Comments.Services;

public interface ICommentService : IBaseService<CommentDTO>
{
    public Task<(int totalItems, List<CommentDTO> listDTOs)> GetManyByProductAsync(Guid productId, int pageNumber, int limit);
}