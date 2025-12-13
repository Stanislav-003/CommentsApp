using backend.Abstractions.Messaging;

namespace backend.Features.Comments.UploadFile;

public record UploadFileCommand(
    Guid CommentId,
    IFormFile? Picture,
    IFormFile? TextFile) : ICommand<Guid>;
