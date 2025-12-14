using backend.Abstractions.Messaging;
using backend.Contracts.Responses;
using backend.Database;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.Comments.DownloadFile;

public class DownloadFileQueryHandler : IQueryHandler<DownloadFileQuery, FileResponse>
{
    private readonly BlobStorageService _blobService;
    private readonly ApplicationDbContext _dbContext;

    public DownloadFileQueryHandler(BlobStorageService blobService, ApplicationDbContext dbContext)
    {
        _blobService = blobService;
        _dbContext = dbContext;
    }

    public async Task<FileResponse> Handle(DownloadFileQuery query, CancellationToken cancellationToken)
    {
        var comment = await _dbContext.Comments
            .Include(c => c.Attachment)
            .FirstOrDefaultAsync(c => c.Id == query.commentId, cancellationToken);

        if (comment == null)
            throw new ApplicationException("Comment not found");

        if (comment.Attachment == null || string.IsNullOrEmpty(comment.Attachment.FileUrl))
            throw new ApplicationException("Attachment not found");

        var blobData = await _blobService.DownAsync(comment.Attachment.FileName, cancellationToken);

        return blobData;
    }
}
