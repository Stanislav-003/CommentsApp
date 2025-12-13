using backend.Abstractions.Messaging;
using backend.Contracts.Responses;
using backend.Database;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Features.Comments.GetComments;

public class GetCommentsQueryHandler : IQueryHandler<GetCommentsQuery, List<CommentsResponse>>
{
    private readonly ApplicationDbContext _context;

    public GetCommentsQueryHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<CommentsResponse>> Handle(GetCommentsQuery query, CancellationToken cancellationToken)
    {
        // стягую всі коментарі які є
        var commentsQuery = _context.Comments
            .AsNoTracking()
            .AsSplitQuery() // дає перформанс при великій кількості вкладених include
            .Include(c => c.User)
                .ThenInclude(u => u.Credential)
            .Include(c => c.Attachment)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Username))
            commentsQuery = commentsQuery.Where(c => c.User.FirstName.Contains(query.Username));

        if (!string.IsNullOrWhiteSpace(query.Email))
            commentsQuery = commentsQuery.Where(c => c.User.Credential != null && c.User.Credential.Email.Contains(query.Email));

        var comments = await commentsQuery.ToListAsync(cancellationToken);

        // будую дерево в пам'яті 
        var lookup = comments.ToDictionary(c => c.Id);
        
        var rootComments = new List<Comment>();

        foreach (var comment in comments)
        {
            if (comment.ParentId.HasValue && lookup.TryGetValue(comment.ParentId.Value, out var parent))
                parent.Children.Add(comment);
            else
                rootComments.Add(comment);
        }

        var page = Math.Max(1, query.Page);
        var pageSize = Math.Clamp(query.PageSize, 1, 100);

        IOrderedEnumerable<Comment> orderedRoots;

        if (query.SortAscending)
            orderedRoots = rootComments.OrderBy(c => c.CreatedAt);
        else
            orderedRoots = rootComments.OrderByDescending(c => c.CreatedAt);

        var pagedRoots = orderedRoots
            .Skip((page - 1) * pageSize)
            .Take(pageSize);

        return pagedRoots
            .Select(MapToResponse)
            .ToList();
    }
    
    private CommentsResponse MapToResponse(Comment c)
    {
        return new CommentsResponse(
            c.Id,
            c.Text,
            c.User.FirstName,
            c.User.Credential?.Email ?? "",
            c.Attachment?.FileUrl ?? "",
            c.CreatedAt,
            c.Children
                .Select(MapToResponse)
                .ToList()
        );
    }
}