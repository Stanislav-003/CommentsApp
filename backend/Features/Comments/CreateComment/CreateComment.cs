using backend.Abstractions;
using backend.Abstractions.Messaging;
using backend.Contracts.Dtos.Comments;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace backend.Features.Comments.CreateComment;

public class CreateComment : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/comments/create", async (
            CreateCommentDto dto,
            ClaimsPrincipal user,
            ICommandHandler<CreateCommentCommand, Guid> handler,
            CancellationToken ct) =>
        {
            var userIdClaim = user.FindFirst("UserId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                throw new ApplicationException("UserIsNotAuthorize");

            var command = new CreateCommentCommand(
                dto.ParentId,
                userId,
                dto.CaptchaCode,
                dto.CaptchaId,
                dto.Text);

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