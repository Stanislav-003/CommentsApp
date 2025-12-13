namespace backend.Contracts.Responses;

public record GetCaptchaResponse(
    string CaptchaId,
    string CaptchaImage);