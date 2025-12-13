using backend.Abstractions;
using backend.Abstractions.Messaging;
using backend.Contracts.Dtos.Comments;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Features.Comments.UploadFile;

public class UploadFile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/comments/file-upload", async (
            [FromQuery] Guid CommentId,
            IFormFile? Picture,
            IFormFile? TextFile,
            ICommandHandler <UploadFileCommand, Guid> handler,
            CancellationToken ct) =>
        {
            var command = new UploadFileCommand(
                CommentId,
                Picture,
                TextFile);

            var result = await handler.Handle(command, ct);

            return result;
        })
        .RequireAuthorization(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(UsersRoles.USER)
        .Build())
        .Accepts<IFormFile>("multipart/form-data")
        .WithTags("Comments")
        .DisableAntiforgery();
    }
}