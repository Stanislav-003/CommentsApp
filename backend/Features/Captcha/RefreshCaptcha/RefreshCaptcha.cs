using backend.Abstractions;
using backend.Abstractions.Messaging;
using backend.Contracts.Responses;

namespace backend.Features.Captcha.RefreshCaptcha;

public class RefreshCaptcha : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/captcha/refresh", (
            string CaptchaId,
            IQueryHandler<RefreshCaptchaQuery, GetCaptchaResponse> handler, 
            CancellationToken ct) =>
        {
            var query = new RefreshCaptchaQuery(CaptchaId);

            var result = handler.Handle(query, ct);
            return result;
        })
        .WithTags("Captcha");
    }
}