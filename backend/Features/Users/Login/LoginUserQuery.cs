using backend.Abstractions.Messaging;
using backend.Contracts.Responses;

namespace backend.Features.Users.Login;

public record LoginUserQuery(
    string Email,
    string Password) : IQuery<LoginResponse>;