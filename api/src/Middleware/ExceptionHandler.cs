using Microsoft.AspNetCore.Diagnostics;

namespace Api.Middleware;

public static class ExceptionHandler
{
    public static void UseExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(LogException));
    }

    public static async Task LogException(HttpContext httpContext)
    {
        IExceptionHandlerPathFeature? exceptionHandlerPathFeature = httpContext.Features.Get<IExceptionHandlerPathFeature>();
        if (exceptionHandlerPathFeature?.Error is BadHttpRequestException)
        {
            await Results
                .Problem(
                    statusCode: StatusCodes.Status400BadRequest, 
                    detail: "Request was not formatted properly"
                )
                .ExecuteAsync(httpContext);
        }
        else
        {
            await Results.Problem().ExecuteAsync(httpContext);
        }
    }
}
