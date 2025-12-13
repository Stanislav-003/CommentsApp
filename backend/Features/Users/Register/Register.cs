using backend.Abstractions;
using backend.Abstractions.Messaging;
using backend.Contracts.Dtos.Users;
using backend.Contracts.Responses;

namespace backend.Features.Users.Register;

public class Register : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users/register", async (
            RegisterUserDto dto,
            ICommandHandler<RegisterUserCommand, RegisterResponse> handler,
            CancellationToken ct) =>
        {
            var command = new RegisterUserCommand(
                dto.Email,
                dto.Password,
                dto.ConfirmPassword,
                dto.FirstName,
                dto.LastName,
                dto.role);

            var result = await handler.Handle(command, ct);

            return result;
        })
        .WithTags("Users");
    }
}
