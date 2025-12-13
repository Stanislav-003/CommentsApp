namespace frontend.Responses;

public record FileResponse(byte[] stream, string contentType);