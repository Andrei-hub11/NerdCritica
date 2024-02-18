using NerdCritica.Domain.Utils;

namespace NerdCritica.Domain.Entities;

public class Comment
{
    public Guid CommentId { get; private set; }
    public Guid RatingId { get; private set; }
    public string IdentityUserId { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;

    private Comment(Guid commentId, Guid ratingId, string identityUserId, string content)
    {
        CommentId = commentId;
        RatingId = ratingId;
        IdentityUserId = identityUserId;
        Content = content;
    }

    public static Result<Comment> Create(Guid commentId, Guid ratingId, string userId, string content)
    {
        var isCreate = true;
        var result = CommentValidation(content, isCreate, ratingId, userId);

        if (result.Count > 0)
        {
            var emptyComment = new Comment(Guid.Empty, Guid.Empty, string.Empty,
                string.Empty);
            return Result.AddErrors(result, emptyComment);
        }

        var comment = new Comment(commentId, ratingId, userId, content);

        return Result.Ok(comment);
    }

    public static Result<Comment> Update(Comment comment, string content)
    {
        var isCreate = false;
        var result = CommentValidation(content, isCreate);

        if (result.Count > 0)
        {
            var emptyComment = new Comment(Guid.Empty, Guid.Empty, string.Empty,
                string.Empty);
            return Result.AddErrors(result, emptyComment);
        }

        comment.Content = content;

        return Result.Ok(comment);
    }

    private static List<Error> CommentValidation(string comment, bool isCreate, 
        Guid? ratingId = null, string identityUserId = "")
    {
        var errors = new List<Error>();

        if (string.IsNullOrWhiteSpace(comment))
        {
            errors.Add(new Error("O comentário do post não pode estar vazio"));
        }

        if (isCreate && ratingId == Guid.Empty)
        {
            errors.Add(new Error("O id da avaliação precisa ser fornecido."));
        }

        if (isCreate && string.IsNullOrWhiteSpace(identityUserId))
        {
            errors.Add(new Error("O id do usuário não pode estar vazio"));
        }

        if (isCreate && !string.IsNullOrEmpty(identityUserId) &&
            !Guid.TryParse(identityUserId, out Guid result))
        {
            errors.Add(new Error($"{identityUserId} não é um id válido."));
        }

        return errors;
    }
}
