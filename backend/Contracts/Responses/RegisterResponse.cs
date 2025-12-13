namespace backend.Contracts.Responses;

public record RegisterResponse(
    string Token, 
    long TokenExpired);
