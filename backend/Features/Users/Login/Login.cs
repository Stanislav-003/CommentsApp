using backend.Abstractions;
using backend.Abstractions.Messaging;
using backend.Contracts.Dtos.Users;
using backend.Contracts.Responses;

namespace backend.Features.Users.Login;

public class Login : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users/login", async (
            LoginDto dto,
            IQueryHandler<LoginUserQuery, LoginResponse> handler,
            CancellationToken ct) =>
        {
            var query = new LoginUserQuery(
                dto.Email,
                dto.Password);
            var result = await handler.Handle(query, ct);
            return result;
        })
        .WithTags("Users");
    }
}