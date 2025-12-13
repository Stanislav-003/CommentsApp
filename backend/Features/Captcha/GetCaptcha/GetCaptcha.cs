using backend.Abstractions;
using backend.Abstractions.Messaging;
using backend.Contracts.Responses;
using backend.Services;

namespace backend.Features.Captcha.GetCaptcha;

public class GetCaptcha : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/captcha/generate", (
            IQueryHandler<GetCaptchaQuery, GetCaptchaResponse> handler,
            CancellationToken ct) =>
        {
            var query = new GetCaptchaQuery();            
            
            var result = handler.Handle(query, ct);
            return result;
        })
        .WithTags("Captcha");
    }
}