using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;

using FluentValidation.Results;

using Api.Common.Constants;
using Api.Common.Extensions;
using Api.Common.Services;
using Api.Common.Types;

namespace Api.Features.Auth.Logout;

public class Handler
{
    public static async Task<IResult> HandleAsync(
        ClaimsPrincipal user,
        [FromBody] Request request,
        [FromServices] RequestValidator requestValidator,
        [FromServices] Service service,
        ILogger<Handler> logger,
        CancellationToken cancellationToken
    )
    {
        long userId = user.GetUserId();

        logger.LogInformation("Logging out session {SessionId}", request.SessionId);
        ValidationResult validationResult = await requestValidator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
        {
            logger.LogInformation("Request validation failed: {Error}", validationResult.ToString());
            return validationResult.AsHttpError();
        }

        Result result = await service.LogoutAsync(userId, request.SessionId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Could not logout. Reason: {Reason}", result.Error!.Message);
            return result.Error.AsHttpError();
        }

        logger.LogInformation("Session logged out successfully");
        return result.AsHttpResponse();
    }

    public static async Task<IResult> HandleWebAsync(
        ClaimsPrincipal user,
        HttpRequest httpRequest,
        HttpResponse httpResponse,
        [FromServices] Service service,
        [FromServices] ICookieService cookieService,
        ILogger<Handler> logger,
        CancellationToken cancellationToken
    )
    {
        long userId = user.GetUserId();
        string? sessionId = httpRequest.Cookies[TokenType.SessionId.Name];
        if (sessionId is null)
        {
            return Results.Unauthorized();
        }

        logger.LogInformation("Logging out session {SessionId}", sessionId);

        Result result = await service.LogoutAsync(userId, sessionId, cancellationToken);
        if (!result.IsSuccess)
        {
            logger.LogError("Could not logout. Reason: {Reason}", result.Error!.Message);
            return result.Error.AsHttpError();
        }

        cookieService.DeleteCookie(httpResponse, TokenType.SessionId.Name);
        cookieService.DeleteCookie(httpResponse, TokenType.RefreshToken.Name);

        logger.LogInformation("Session logged out successfully");
        return result.AsHttpResponse();
    }
}
