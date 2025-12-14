using backend.Abstractions.Messaging;
using backend.Database;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.Comments.DeleteComment;

public class DeleteCommentCommandHandler : ICommandHandler<DeleteCommentCommand, Guid>
{
    private readonly ApplicationDbContext _dbContext;

    public DeleteCommentCommandHandler(ApplicationDbContext context)
    {
        _dbContext = context;
    }

    public async Task<Guid> Handle(DeleteCommentCommand command, CancellationToken cancellationToken)
    {
        var comment = await _dbContext.Comments
            .FirstOrDefaultAsync(c => c.Id == command.CommentId, cancellationToken);

        if (comment is null)
            throw new ApplicationException("CommentNotFound");

        _dbContext.Comments.Remove(comment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return comment.Id;
    }
}
