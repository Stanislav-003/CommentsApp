namespace frontend.Requests;

public record CreateCommentRequest(
    Guid? ParentId,
    string Text,
    string CaptchaCode,
    string CaptchaId);