using App.Areas.Comments.Models;

namespace App.Areas.Comments.Repositories;

public interface ICommentRepository
{
    public Task<List<CommentModel>> GetManyByProductAsync(Guid productId, int pageNumber, int limit);

    public Task<CommentModel> GetOneByIdAsync(Guid id);

    public Task<int> GetTotalByProductAsync(Guid productId);

    public Task<int> CreateAsync(CommentModel comment);

    public Task<int> DeleteAsync(CommentModel comment);
}