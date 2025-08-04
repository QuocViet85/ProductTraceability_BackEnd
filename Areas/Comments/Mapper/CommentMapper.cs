using App.Areas.Comments.Models;

namespace App.Areas.Comments.Mapper;

public static class CommentMapper
{
    public static CommentDTO ModelToDto(CommentModel comment)
    {
        return new CommentDTO()
        {
            Id = comment.Id,
            Content = comment.Content,
            ProductId = comment.ProductId,
            CreatedAt = comment.CreatedAt
        };
    }
    public static CommentModel DtoToModel(CommentDTO commentDTO, CommentModel commentUpdate = null)
    {
        CommentModel comment;
        if (commentUpdate == null)
        {
            comment = new CommentModel();
        }
        else
        {
            comment = commentUpdate;
        }

        comment.ProductId = commentDTO.ProductId;
        comment.Content = commentDTO.Content;
        return comment;
    }
}