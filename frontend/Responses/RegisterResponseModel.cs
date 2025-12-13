namespace frontend.Responses;

public record RegisterResponseModel(
    string Token,
    long TokenExpired);