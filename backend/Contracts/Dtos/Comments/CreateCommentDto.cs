namespace backend.Contracts.Dtos.Comments;

public record CreateCommentDto(
    Guid? ParentId,
    string CaptchaCode,
    string CaptchaId,
    string Text);