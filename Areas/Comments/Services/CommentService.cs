using System.Diagnostics;
using System.Security.Claims;
using App.Areas.Auth.AuthorizationType;
using App.Areas.Auth.Mapper;
using App.Areas.Comments.Mapper;
using App.Areas.Comments.Models;
using App.Areas.Comments.Repositories;
using App.Areas.Products.Repositories;
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
        List<CommentDTO> commentDTOs = new List<CommentDTO>();
        foreach (var comment in listComments)
        {
            var commentDTO = CommentMapper.ModelToDto(comment);
            AddRelationToDTO(commentDTO, comment);
            commentDTOs.Add(commentDTO);
        }

        return (totalComments, commentDTOs);
    }
    public async Task CreateAsync(CommentDTO commentDTO, ClaimsPrincipal userNowFromJwt)
    {
        bool existProduct = await _productRepo.CheckExistById(commentDTO.ProductId);
        if (!existProduct)
        {
            throw new Exception("Sản phẩm không tồn tại");
        }

        var comment = CommentMapper.DtoToModel(commentDTO);
        var userIdNow = userNowFromJwt.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        comment.CreatedUserId = userIdNow;
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
        if (userNowFromJwt.IsInRole(Roles.ADMIN) || comment.CreatedUserId == userIdNow)
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
}