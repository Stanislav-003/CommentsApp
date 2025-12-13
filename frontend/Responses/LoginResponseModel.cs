namespace frontend.Responses;

public record LoginResponseModel(
    string Token, 
    long TokenExpired);