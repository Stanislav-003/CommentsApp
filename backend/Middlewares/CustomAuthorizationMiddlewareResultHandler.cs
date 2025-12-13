using backend.Shared.Errors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace backend.Middlewares;

public class CustomAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

    public async Task HandleAsync(RequestDelegate next, HttpContext context, AuthorizationPolicy policy, PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Forbidden)
            throw new AppException("AccessForbidden");
        else if (authorizeResult.Challenged)
            throw new AppException("UserIsNotAuthorized");

        await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }
}