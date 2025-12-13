using backend.Abstractions;
using backend.Abstractions.Messaging;
using backend.Contracts.Responses;
using backend.Models;
using Microsoft.AspNetCore.Authorization;

namespace backend.Features.Comments.GetComments;

public class GetComments : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/comments/get-all-comments", async (
            string? username,
            string? email,
            bool sortAscending,
            int page,
            int pageSize,
            IQueryHandler <GetCommentsQuery, List<CommentsResponse>> handler,
            CancellationToken ct) =>
        {
            var query = new GetCommentsQuery(username, email, sortAscending, page, pageSize);
            var result = await handler.Handle(query, ct);

            return result;
        })
        .RequireAuthorization(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(UsersRoles.USER)
        .Build())
        .WithTags("Comments");
    }
}