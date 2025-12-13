namespace backend.Contracts.Dtos.Users;

public record LoginDto(
    string Email,
    string Password);