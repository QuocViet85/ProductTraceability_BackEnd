using System.Security.Claims;
using App.Areas.Comments.Models;

namespace App.Areas.Comments.Services;

public interface ICommentService
{
    public Task<(int totalItems, List<CommentDTO> listDTOs)> GetManyByProductAsync(Guid productId, int pageNumber, int limit);

    public Task CreateAsync(CommentDTO comment, ClaimsPrincipal userNowFromJwt);

    public Task DeleteAsync(Guid id, ClaimsPrincipal userNowFromJwt);
}