using backend.Abstractions;
using backend.Abstractions.Messaging;
using backend.Contracts.Responses;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.DownloadFile;

public class DownloadFile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/comments/file-download", async (
            Guid commentId,
            IQueryHandler<DownloadFileQuery, FileResponse> handler,
            CancellationToken ct) =>
        {
            var file = await handler.Handle(new DownloadFileQuery(commentId), ct);

            return Results.File(
                file.stream,
                file.contentType);
        })
        //.RequireAuthorization(new AuthorizationPolicyBuilder()
        //.RequireAuthenticatedUser()
        //.RequireRole(UsersRoles.USER)
        //.Build())
        .WithTags("Comments");
    }
}