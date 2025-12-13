using backend.Models;

namespace backend.Contracts.Dtos.Users;

public record RegisterUserDto(
    string Email,
    string Password,
    string ConfirmPassword,
    string FirstName,
    string LastName,
    CredentialRole role);