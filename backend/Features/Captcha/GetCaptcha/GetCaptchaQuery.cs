using backend.Abstractions.Messaging;
using backend.Contracts.Responses;

namespace backend.Features.Captcha.GetCaptcha;

public record GetCaptchaQuery : IQuery<GetCaptchaResponse>;