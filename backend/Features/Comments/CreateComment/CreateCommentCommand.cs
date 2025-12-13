using backend.Abstractions.Messaging;

namespace backend.Features.Comments.CreateComment;

public record CreateCommentCommand(
    Guid? ParentId,
    Guid UserId,
    string CaptchaCode,
    string CaptchaId,
    string Text) : ICommand<Guid>;