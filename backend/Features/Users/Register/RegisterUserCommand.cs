using backend.Abstractions.Messaging;
using backend.Contracts.Responses;
using backend.Models;

namespace backend.Features.Users.Register;

public record RegisterUserCommand(
    string Email,
    string Password,
    string ConfirmPassword,
    string FirstName,
    string LastName,
    CredentialRole Role) : ICommand<RegisterResponse>;
