namespace backend.Contracts.Responses;

public record FileResponse(byte[] stream, string contentType);