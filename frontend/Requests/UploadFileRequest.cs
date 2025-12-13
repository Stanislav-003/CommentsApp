namespace frontend.Requests;

public record UploadFileRequest(
    Guid CommentId,
    IFormFile? Picture,
    IFormFile? TextFile);