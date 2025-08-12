using System.Diagnostics;
using System.Security.Claims;
using App.Areas.Auth.AuthorizationType;
using App.Areas.Auth.Mapper;
using App.Areas.Comments.Mapper;
using App.Areas.Comments.Models;
using App.Areas.Comments.Repositories;
using App.Areas.SanPham.Repositories;
using Areas.Auth.DTO;

namespace App.Areas.Comments.Services;

public class CommentService : ICommentService
{
    private readonly ICommentRepository _commentRepo;
    private readonly IProductRepository _productRepo;

    public CommentService(ICommentRepository commentRepo, IProductRepository productRepo)
    {
        _commentRepo = commentRepo;
        _productRepo = productRepo;
    }

    public async Task<(int totalItems, List<CommentDTO> listDTOs)> GetManyByProductAsync(Guid productId, int pageNumber, int limit)
    {
        int totalComments = await _commentRepo.GetTotalByProductAsync(productId);
        Paginate.SetPaginate(ref pageNumber, ref limit);

        List<CommentModel> listComments = await _commentRepo.GetManyByProductAsync(productId, pageNumber, limit);
        List<CommentDTO> listCommentDTOs = new List<CommentDTO>();
        foreach (var comment in listComments)
        {
            var commentDTO = CommentMapper.ModelToDto(comment);
            AddRelationToDTO(commentDTO, comment);
            listCommentDTOs.Add(commentDTO);
        }

        return (totalComments, listCommentDTOs);
    }
    public async Task CreateAsync(CommentDTO commentDTO, ClaimsPrincipal userNowFromJwt)
    {
        bool existProduct = await _productRepo.CheckExistByIdAsync(commentDTO.ProductId);
        if (!existProduct)
        {
            throw new Exception("Sản phẩm không tồn tại");
        }

        var comment = CommentMapper.DtoToModel(commentDTO);
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        comment.CreatedUserId = Guid.Parse(userIdNow);
        comment.CreatedAt = DateTime.Now;

        int result = await _commentRepo.CreateAsync(comment);

        if (result == 0)
        {
            throw new Exception("Lỗi cơ sở dữ liệu. Tạo bình luận thất bại");
        }
    }

    public async Task DeleteAsync(Guid id, ClaimsPrincipal userNowFromJwt)
    {
        var comment = await _commentRepo.GetOneByIdAsync(id);

        if (comment == null)
        {
            throw new Exception("Bình luận không tồn tại");
        }

        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userNowFromJwt.IsInRole(Roles.ADMIN) || comment.CreatedUserId.ToString() == userIdNow)
        {
            int result = await _commentRepo.DeleteAsync(comment);

            if (result == 0)
            {
                throw new Exception("Lỗi cơ sở dữ liệu. Tạo bình luận thất bại");
            }
        }
        else
        {
            throw new UnauthorizedAccessException("Không có quyền xóa bình luận");
        }
    }

    private void AddRelationToDTO(CommentDTO commentDTO, CommentModel comment)
    {
        if (comment.CreatedUser != null)
        {
            commentDTO.CreatedUser = UserMapper.ModelToDto(comment.CreatedUser);
        }
    }

    public Task<(int totalItems, List<CommentDTO> listDTOs)> GetManyAsync(int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }

    public Task<CommentDTO> GetOneByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<(int totalItems, List<CommentDTO> listDTOs)> GetMyManyAsync(ClaimsPrincipal userNowFromJwt, int pageNumber, int limit, string search, bool descending)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Guid id, CommentDTO TDto, ClaimsPrincipal userNowFromJwt)
    {
        throw new NotImplementedException();
    }
}