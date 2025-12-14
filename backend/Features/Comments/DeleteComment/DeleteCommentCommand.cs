using backend.Abstractions.Messaging;

namespace backend.Features.Comments.DeleteComment;

public record DeleteCommentCommand(Guid CommentId, Guid UserId) : ICommand<Guid>;
