using backend.Abstractions;
using backend.Abstractions.Messaging;
using backend.Contracts.Dtos.Comments;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace backend.Features.Comments.DeleteComment;

public class DeleteComment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/comments/delete", async (
            DeleteCommentDto dto,
            ClaimsPrincipal user,
            ICommandHandler<DeleteCommentCommand, Guid> handler,
            CancellationToken ct) =>
        {
            var userIdClaim = user.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                throw new ApplicationException("UserIsNotAuthorize");

            var command = new DeleteCommentCommand(dto.CommentId, userId);

            var result = await handler.Handle(command, ct);

            return result;
        })
        .RequireAuthorization(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .RequireRole(UsersRoles.USER)
        .Build())
        .WithTags("Comments");
    }
}