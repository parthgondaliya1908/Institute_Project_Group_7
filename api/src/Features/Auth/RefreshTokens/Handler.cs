using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using FluentValidation;
using FluentValidation.Results;

using Api.Common.Extensions;
using Api.Common.Types;
using Api.Common.Options;
using Api.Common.Constants;
using Api.Common.Utils;
using Api.Common.Services;

namespace Api.Features.Auth.RefreshTokens;

public class Handler
{
    public static async Task<IResult> HandleAsync(
        ClaimsPrincipal user,
        [FromBody] Request request,
        [FromServices] IValidator<Request> requestValidator,
        [FromServices] Service service,
        CancellationToken cancellationToken
    )
    {
        long userId = user.GetUserId();

        ValidationResult validationResult = await requestValidator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.AsHttpError();
        }

        Result<GeneratedTokens> result = await service.GenerateNewTokensAsync(userId, request.RefreshToken, request.SessionId, cancellationToken);
        return result.AsHttpResponse();
    }

    public static async Task<IResult> HandleWebAsync(
        HttpContext http,
        [FromServices] Service service,
        [FromServices] ICookieService cookieService,
        [FromServices] IOptions<JwtOptions> jwtOptions,
        CancellationToken cancellationToken
    )
    {
        long userId = http.User.GetUserId();

        string? refreshToken = http.Request.Cookies[TokenType.RefreshToken.Name];
        if (refreshToken is null)
        {
            return Results.Unauthorized();
        }

        string? sessionId = http.Request.Cookies[TokenType.SessionId.Name];
        if (sessionId is null)
        {
            return Results.Unauthorized();
        }

        Result<GeneratedTokens> result = await service.GenerateNewTokensAsync(userId, refreshToken, sessionId, cancellationToken);
        if (!result.IsSuccess)
        {
            return result.Error.AsHttpError();
        }

        GeneratedTokens generatedTokens = result.Value!;
        cookieService.AppendCookie(
            http.Response,
            TokenType.RefreshToken.Name,
            generatedTokens.RefreshToken,
            path: Api.BaseUrl,
            DateTimeOffset.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpiryInDays)
        );

        return Results.Ok(new OnlyAccessToken(generatedTokens.AccessToken));
    }
}
