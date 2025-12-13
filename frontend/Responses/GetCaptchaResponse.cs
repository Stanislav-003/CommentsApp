namespace frontend.Responses;

public record GetCaptchaResponse(
    string CaptchaId,
    string CaptchaImage);