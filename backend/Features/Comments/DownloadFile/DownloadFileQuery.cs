using backend.Abstractions.Messaging;
using backend.Contracts.Responses;

namespace backend.Features.Comments.DownloadFile;

public record DownloadFileQuery(Guid commentId) : IQuery<FileResponse>;