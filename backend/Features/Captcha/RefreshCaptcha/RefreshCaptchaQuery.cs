using backend.Abstractions.Messaging;
using backend.Contracts.Responses;

namespace backend.Features.Captcha.RefreshCaptcha;

public record RefreshCaptchaQuery(string CaptchaId) : IQuery<GetCaptchaResponse>;
