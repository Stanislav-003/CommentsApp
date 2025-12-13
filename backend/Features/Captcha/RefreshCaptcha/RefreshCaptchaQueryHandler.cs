using backend.Abstractions.Messaging;
using backend.Contracts.Responses;
using backend.Helpers;
using Microsoft.Extensions.Caching.Memory;

namespace backend.Features.Captcha.RefreshCaptcha;

public class RefreshCaptchaQueryHandler : IQueryHandler<RefreshCaptchaQuery, GetCaptchaResponse>
{
    private readonly IMemoryCache _cache;

    public RefreshCaptchaQueryHandler(IMemoryCache cache)
    {
        _cache = cache;
    }

    public Task<GetCaptchaResponse> Handle(RefreshCaptchaQuery query, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(query.CaptchaId))
            throw new ApplicationException("CaptchaIdIsRequired");

        _cache.Remove(query.CaptchaId);

        var newCode = CaptchaHelper.GenerateCaptchaCode(6);
        _cache.Set(query.CaptchaId, newCode, TimeSpan.FromMinutes(10));

        var imageBytes = CaptchaHelper.GenerateCaptchaImage(newCode);
        var base64 = Convert.ToBase64String(imageBytes);

        return Task.FromResult(new GetCaptchaResponse(
            query.CaptchaId,
            $"data:image/png;base64,{base64}"));
    }
}
