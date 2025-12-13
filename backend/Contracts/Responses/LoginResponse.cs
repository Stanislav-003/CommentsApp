namespace backend.Contracts.Responses;

public record LoginResponse(
    string Token, 
    long TokenExpired);
