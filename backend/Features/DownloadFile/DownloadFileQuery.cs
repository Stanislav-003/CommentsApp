using backend.Abstractions.Messaging;
using backend.Contracts.Responses;

namespace backend.Features.DownloadFile;

public record DownloadFileQuery(Guid commentId) : IQuery<FileResponse>;