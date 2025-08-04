using App.Areas.Comments.Models;
using App.Database;

namespace App.Areas.Comments.Repositories;

public interface ICommentRepository : IBaseRepository<CommentModel>
{
    public Task<List<CommentModel>> GetManyByProductAsync(Guid productId, int pageNumber, int limit);
    public Task<int> GetTotalByProductAsync(Guid productId);
    public Task<int> CreateAsync(CommentModel comment);
    public Task<int> DeleteAsync(CommentModel comment);
}