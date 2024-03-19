using NerdCritica.Domain.Utils;

namespace NerdCritica.Domain.Entities;

public class Comment
{
    public Guid RatingId { get; private set; }
    public string IdentityUserId { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;

    private Comment(Guid ratingId, string identityUserId, string content)
    {
        RatingId = ratingId;
        IdentityUserId = identityUserId;
        Content = content;
    }

    private Comment(string identityUserId, string content)
    {
        IdentityUserId = identityUserId;
        Content = content;
    }

    public static Result<Comment> Create(Guid ratingId, string identityUserId, string content)
    {
        var isCreate = true;
        var result = CommentValidation(content, isCreate, identityUserId, ratingId);

        if (result.Count > 0)
        {
            var emptyComment = new Comment(Guid.Empty, string.Empty,
                string.Empty);
            return Result.AddErrors(result, emptyComment);
        }

        var comment = new Comment(ratingId, identityUserId, content);

        return Result.Ok(comment);
    }

    public static Result<Comment> From(string identityUserId, string content)
    {
        var isCreate = false;
        var result = CommentValidation(content, isCreate, identityUserId);

        if (result.Count > 0)
        {
            var emptyComment = new Comment(Guid.Empty, string.Empty,
                string.Empty);
            return Result.AddErrors(result, emptyComment);
        }

        var newComment = new Comment(identityUserId, content);

        return Result.Ok(newComment);
    }

    private static List<Error> CommentValidation(string comment, bool isCreate, string identityUserId,
        Guid? ratingId = null)
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

        if (string.IsNullOrWhiteSpace(identityUserId))
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
