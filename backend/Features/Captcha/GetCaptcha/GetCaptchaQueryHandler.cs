using backend.Abstractions.Messaging;
using backend.Contracts.Responses;
using backend.Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace backend.Features.Captcha.GetCaptcha;

public class GetCaptchaQueryHandler : IQueryHandler<GetCaptchaQuery, GetCaptchaResponse>
{
    private readonly IMemoryCache _cache;

    public GetCaptchaQueryHandler(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<GetCaptchaResponse> Handle(GetCaptchaQuery query, CancellationToken cancellationToken)
    {
        var captchaCode = CaptchaHelper.GenerateCaptchaCode(6);
        var captchaId = Guid.NewGuid().ToString();

        _cache.Set(captchaId, captchaCode, TimeSpan.FromMinutes(10));

        var imageBytes = CaptchaHelper.GenerateCaptchaImage(captchaCode);
        var base64 = Convert.ToBase64String(imageBytes);

        return Task.FromResult(new GetCaptchaResponse(
            captchaId,
            $"data:image/png;base64,{base64}"));
    }
}