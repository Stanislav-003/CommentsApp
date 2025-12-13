using backend.Abstractions.Messaging;
using backend.Database;
using backend.Models;
using Microsoft.Extensions.Caching.Memory;

namespace backend.Features.Comments.CreateComment;

public class CreateCommentCommandHandler : ICommandHandler<CreateCommentCommand, Guid>
{
    private readonly IMemoryCache _cache;
    private readonly ApplicationDbContext _dbContext;

    public CreateCommentCommandHandler(IMemoryCache cache,
        ApplicationDbContext dbContext)
    {
        _cache = cache;
        _dbContext = dbContext;
    }

    public async Task<Guid> Handle(CreateCommentCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.CaptchaId) || !_cache.TryGetValue(command.CaptchaId, out string? storedCaptchaCode))
            throw new ApplicationException("CaptchaHasExpiredOrIsInvalidPleaseRefreshAndTryAgain");

        if (storedCaptchaCode != command.CaptchaCode)
            throw new ApplicationException("InvalidCAPTCHAPleaseTryAgain");

        _cache.Remove(command.CaptchaId);

        var comment = new Comment
        {
            ParentId = command.ParentId,
            UserId = command.UserId,
            Text = command.Text,
        };

        await _dbContext.Comments.AddAsync(comment, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return comment.Id;
    }
}