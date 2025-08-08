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
        return await _dbContext.Comments.Where(c => c.ProductId == productId).OrderByDescending(c => c.CreatedAt).Include(c => c.CreatedUser).ToListAsync();
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

    public async Task<bool> CheckExistByIdAsync(Guid id)
    {
        return await _dbContext.Comments.AnyAsync(c => c.Id == id);
    }

    //Not Implement

    public Task<List<CommentModel>> GetManyAsync(int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetTotalAsync()
    {
        throw new NotImplementedException();
    }

    public Task<int> GetMyTotalAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task<List<CommentModel>> GetMyManyAsync(string userId, int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }

    public Task<int> UpdateAsync(CommentModel model)
    {
        throw new NotImplementedException();
    }

    public Task<CommentModel> GetOneByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}