using backend.Abstractions.Messaging;
using backend.Database;
using backend.Models;
using backend.Services;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace backend.Features.Comments.UploadFile;

public class UploadFileCommandHandler : ICommandHandler<UploadFileCommand, Guid>
{
    private readonly ApplicationDbContext _dbContext;
    private readonly BlobStorageService _blobService;

    private const int MaxImageWidth = 320;
    private const int MaxImageHeight = 240;
    private const int MaxTextFileSize = 100 * 1024; // 100 KB

    private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
    private static readonly string[] AllowedTextExtensions = { ".txt" };

    public UploadFileCommandHandler(ApplicationDbContext dbContext, BlobStorageService blobService)
    {
        _dbContext = dbContext;
        _blobService = blobService;
    }

    public async Task<Guid> Handle(UploadFileCommand command, CancellationToken cancellationToken)
    {
        if (command.Picture is null && command.TextFile is null)
            throw new ApplicationException("YouMustAttachEitherAPictureOrATextFile");

        if (command.Picture is not null && command.TextFile is not null)
            throw new ApplicationException("YouMustAttachOnlyPictureOrTextFile");

        var comment = await _dbContext.Comments
            .Include(c => c.Attachment)
            .FirstOrDefaultAsync(c => c.Id == command.CommentId, cancellationToken);

        if (comment == null)
            throw new ApplicationException("CommentNotFound");

        IFormFile file = command.Picture ?? command.TextFile!;
        AttachmentType fileType = command.Picture != null ? AttachmentType.Image : AttachmentType.TextFile;

        switch (fileType)
        {
            case AttachmentType.Image:
                {
                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    
                    if (!AllowedImageExtensions.Contains(ext))
                        throw new ApplicationException("AllowedImageFormatsAre_JPG_PNG_GIF");

                    using var image = await Image.LoadAsync(file.OpenReadStream(), cancellationToken);

                    if (image.Width > MaxImageWidth || image.Height > MaxImageHeight)
                    {
                        var ratioX = (float)MaxImageWidth / image.Width;
                        var ratioY = (float)MaxImageHeight / image.Height;
                        var ratio = Math.Min(ratioX, ratioY);

                        var newWidth = (int)(image.Width * ratio);
                        var newHeight = (int)(image.Height * ratio);

                        image.Mutate(x => x.Resize(newWidth, newHeight));

                        var ms = new MemoryStream();
                        
                        var innerExt = ext;

                        switch (innerExt)
                        {
                            case ".jpg":
                                await image.SaveAsJpegAsync(ms, cancellationToken);
                                break;
                            case ".jpeg":
                                await image.SaveAsJpegAsync(ms, cancellationToken);
                                break;
                            case ".png":
                                await image.SaveAsPngAsync(ms, cancellationToken);
                                break;
                            case ".gif":
                                await image.SaveAsGifAsync(ms, cancellationToken);
                                break;
                            default:
                                throw new ApplicationException("UnsupportedImageFormat");
                        }    
                        
                        ms.Position = 0;

                        file = new FormFile(ms, 0, ms.Length, file.Name, file.FileName)
                        {
                            Headers = file.Headers,
                            ContentType = file.ContentType
                        };
                    }
                    break;
                }

            case AttachmentType.TextFile:
                {
                    var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                    
                    if (!AllowedTextExtensions.Contains(ext))
                        throw new ApplicationException("Only_TXT_FilesAreAllowed");

                    if (file.Length > MaxTextFileSize)
                        throw new ApplicationException("TextFileSizeMustNotExceed_100KB");

                    break;
                }
            default:
                throw new ApplicationException("UnsupportedFileType");
        }

        var url = await _blobService.UploadFileAsync(file, cancellationToken);

        if (comment.Attachment is not null)
        {
            comment.Attachment.FileUrl = url;
            comment.Attachment.FileName = file.FileName;
            comment.Attachment.FileType = fileType;
        }
        else
        {
            comment.Attachment = new Attachment
            {
                CommentId = comment.Id,
                FileUrl = url,
                FileName = file.FileName,
                FileType = fileType
            };
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return comment.Id;
    }
}