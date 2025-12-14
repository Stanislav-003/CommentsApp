using backend.Abstractions;
using backend.Abstractions.Messaging;
using backend.Contracts.Responses;

namespace backend.Features.Comments.DownloadFile;

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
        .WithTags("Comments");
    }
}