using backend.Abstractions.Messaging;
using backend.Contracts.Responses;

namespace backend.Features.Comments.GetComments;

public record GetCommentsQuery(
    string? Username = null, 
    string? Email = null,
    bool SortAscending = true,
    int Page = 1,
    int PageSize = 100) : IQuery<List<CommentsResponse>>;