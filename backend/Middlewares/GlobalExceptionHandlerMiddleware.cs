using backend.Shared.Errors;
using Microsoft.AspNetCore.Http.Features;
using System.Diagnostics;

namespace backend.Middlewares;

public class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        Activity? activity = context.Features.Get<IHttpActivityFeature>()?.Activity;

        var problemDetails = exception switch
        {
            CustomValidationException validationEx => validationEx.Errors.Select(err => new Error
            {
                Type = validationEx.GetType().Name,
                Title = "Validation exception",
                Status = StatusCodes.Status500InternalServerError,
                Detail = err.Detail,
                TraceId = activity?.Id
            }).ToList(),

            AppException ex => new List<Error>
            {
                new Error
                {
                    Type = ex.GetType().Name,
                    Title = "An error occured",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = ex.Message,
                    TraceId = activity?.Id,
                }
            },

            _ => new List<Error>
            {
                new Error
                {
                    Type = exception.GetType().Name,
                    Title = "An error occured",
                    Status = StatusCodes.Status500InternalServerError,
                    Detail = exception.Message,
                    TraceId = activity?.Id
                }
            }
        };

        context.Response.StatusCode = problemDetails.First().Status;
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
