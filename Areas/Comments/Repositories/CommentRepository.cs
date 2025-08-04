using System.Data.Common;
using App.Areas.Comments.Models;
using App.Database;
using Microsoft.EntityFrameworkCore;

namespace App.Areas.Comments.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly AppDBContext _dbContext;

    public CommentRepository(AppDBContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<CommentModel>> GetManyByProductAsync(Guid productId, int pageNumber, int limit)
    {
        return await _dbContext.Comments.Where(c => c.ProductId == productId).Include(c => c.CreatedUser).Skip((pageNumber - 1) * limit).Take(limit).ToListAsync();
    }

    public async Task<CommentModel> GetOneByIdAsync(Guid id)
    {
        return await _dbContext.Comments.Where(c => c.Id == id).FirstOrDefaultAsync();
    }

    public async Task<int> GetTotalByProductAsync(Guid productId)
    {
        return await _dbContext.Comments.Where(c => c.ProductId == productId).CountAsync();
    }

    public async Task<int> CreateAsync(CommentModel comment)
    {
        _dbContext.Comments.Add(comment);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> DeleteAsync(CommentModel comment)
    {
        _dbContext.Comments.Remove(comment);
        return await _dbContext.SaveChangesAsync();
    }
}