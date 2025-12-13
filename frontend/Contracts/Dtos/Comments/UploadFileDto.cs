namespace backend.Contracts.Dtos.Comments;

public record UploadFileDto(
    Guid CommentId,
    IFormFile? Picture,
    IFormFile? TextFile);